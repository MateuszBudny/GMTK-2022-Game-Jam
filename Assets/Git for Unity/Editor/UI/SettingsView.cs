using Unity.VersionControl.Git;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Editor.Tasks;
using Unity.Editor.Tasks.Logging;
using UnityEditor;
using UnityEngine;

namespace Unity.VersionControl.Git
{
    [Serializable]
    class SettingsView : Subview
    {
        private const string GitRepositoryTitle = "Repository Configuration";
        private const string GitRepositoryRemoteLabel = "Remote";
        private const string GitRepositorySave = "Save Repository";
        private const string GeneralSettingsTitle = "General";
        private const string DebugSettingsTitle = "Debug";
        private const string PrivacyTitle = "Privacy";
        private const string WebTimeoutLabel = "Timeout of web requests";
        private const string GitTimeoutLabel = "Timeout of git commands";
        private const string HierarchyIconsVisiblityToggleLabel = "Show Git status icons in Hierarchy View";
        private const string HierarchyIconsIndentToggleLabel = "Indent Git status icons in Hierarchy View";
        private const string EnableTraceLoggingLabel = "Enable Trace Logging";
        private const string MetricsOptInLabel = "Help us improve by sending anonymous usage data";
        private const string DefaultRepositoryRemoteName = "origin";

        [NonSerialized] private bool currentRemoteHasUpdate;
        [NonSerialized] private bool isBusy;

        [SerializeField] private GitPathView gitPathView = new GitPathView();
        [SerializeField] private bool hasRemote;
        [SerializeField] private CacheUpdateEvent lastCurrentRemoteChangedEvent;
        [SerializeField] private string newRepositoryRemoteUrl;
        [SerializeField] private string repositoryRemoteName;
        [SerializeField] private string repositoryRemoteUrl;
        [SerializeField] private Vector2 scroll;
        [SerializeField] private UserSettingsView userSettingsView = new UserSettingsView();
        [SerializeField] private int webTimeout;
        [SerializeField] private int gitTimeout;
        [SerializeField] private bool areHierarchyIconsTurnedOn;
        [SerializeField] private bool areHierarchyIconsIndented;

        public override void InitializeView(IView parent)
        {
            base.InitializeView(parent);
            gitPathView.InitializeView(this);
            userSettingsView.InitializeView(this);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            gitPathView.OnEnable();
            userSettingsView.OnEnable();
            AttachHandlers(Repository);

            if (Repository != null)
            {
                ValidateCachedData(Repository);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            gitPathView.OnDisable();
            userSettingsView.OnDisable();
            DetachHandlers(Repository);
        }

        public override void OnDataUpdate()
        {
            base.OnDataUpdate();
            userSettingsView.OnDataUpdate();
            gitPathView.OnDataUpdate();

            MaybeUpdateData();
        }

        public override void Refresh()
        {
            base.Refresh();
            gitPathView.Refresh();
            userSettingsView.Refresh();
            Refresh(CacheType.RepositoryInfo);
        }

        public override void OnGUI()
        {
            scroll = GUILayout.BeginScrollView(scroll);
            {
                userSettingsView.OnGUI();

                GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

                if (Repository != null)
                {
                    OnRepositorySettingsGUI();
                    GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                }

                gitPathView.OnGUI();
                OnGeneralSettingsGui();
                OnLoggingSettingsGui();
            }

            GUILayout.EndScrollView();

            DoProgressGUI();
        }

        private void AttachHandlers(IRepository repository)
        {
            if (repository == null)
            {
                return;
            }

            repository.CurrentRemoteChanged += RepositoryOnCurrentRemoteChanged;
        }

        private void RepositoryOnCurrentRemoteChanged(CacheUpdateEvent cacheUpdateEvent)
        {
            if (!lastCurrentRemoteChangedEvent.Equals(cacheUpdateEvent))
            {
                lastCurrentRemoteChangedEvent = cacheUpdateEvent;
                currentRemoteHasUpdate = true;
                Redraw();
            }
        }

        private void DetachHandlers(IRepository repository)
        {
            if (repository == null)
            {
                return;
            }

            repository.CurrentRemoteChanged -= RepositoryOnCurrentRemoteChanged;
        }

        private void ValidateCachedData(IRepository repository)
        {
            repository.CheckAndRaiseEventsIfCacheNewer(CacheType.RepositoryInfo, lastCurrentRemoteChangedEvent);
        }

        private void MaybeUpdateData()
        {
            if (Repository == null)
                return;

            if (currentRemoteHasUpdate)
            {
                currentRemoteHasUpdate = false;
                var currentRemote = Repository.CurrentRemote;
                hasRemote = currentRemote.HasValue && !String.IsNullOrEmpty(currentRemote.Value.Url);
                if (!hasRemote)
                {
                    repositoryRemoteName = DefaultRepositoryRemoteName;
                    newRepositoryRemoteUrl = repositoryRemoteUrl = string.Empty;
                }
                else
                {
                    repositoryRemoteName = currentRemote.Value.Name;
                    newRepositoryRemoteUrl = repositoryRemoteUrl = currentRemote.Value.Url;
                }
            }
        }

        private void OnRepositorySettingsGUI()
        {
            GUILayout.Label(GitRepositoryTitle, EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(IsBusy);
            {
                newRepositoryRemoteUrl = EditorGUILayout.TextField(GitRepositoryRemoteLabel + ": " + repositoryRemoteName, newRepositoryRemoteUrl);
                var needsSaving = newRepositoryRemoteUrl != repositoryRemoteUrl && !String.IsNullOrEmpty(newRepositoryRemoteUrl);
                EditorGUI.BeginDisabledGroup(!needsSaving);
                {
                    if (GUILayout.Button(GitRepositorySave, GUILayout.ExpandWidth(false)))
                    {
                        try
                        {
                            isBusy = true;
                            Repository.SetupRemote(repositoryRemoteName, newRepositoryRemoteUrl)
                                .FinallyInUI((_, __) =>
                                {
                                    isBusy = false;
                                    Redraw();
                                })
                                .Start();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void OnLoggingSettingsGui()
        {
            GUILayout.Label(DebugSettingsTitle, EditorStyles.boldLabel);

            var traceLogging = LogHelper.TracingEnabled;

            EditorGUI.BeginChangeCheck();
            {
                traceLogging = GUILayout.Toggle(traceLogging, EnableTraceLoggingLabel);
            }
            if (EditorGUI.EndChangeCheck())
            {
                LogHelper.TracingEnabled = traceLogging;
                Manager.UserSettings.Set(Constants.TraceLoggingKey, traceLogging);
            }
        }

        private void OnGeneralSettingsGui()
        {
            GUILayout.Label(GeneralSettingsTitle, EditorStyles.boldLabel);

            webTimeout = ApplicationConfiguration.WebTimeout;
            EditorGUI.BeginChangeCheck();
            {
                webTimeout = EditorGUILayout.IntField(WebTimeoutLabel, webTimeout);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ApplicationConfiguration.WebTimeout = webTimeout;
                Manager.UserSettings.Set(Constants.WebTimeoutKey, webTimeout);
            }

            gitTimeout = ApplicationConfiguration.GitTimeout;
            EditorGUI.BeginChangeCheck();
            {
                gitTimeout = EditorGUILayout.IntField(GitTimeoutLabel, gitTimeout);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ApplicationConfiguration.GitTimeout = gitTimeout;
                Manager.UserSettings.Set(Constants.GitTimeoutKey, gitTimeout);
            }

            areHierarchyIconsTurnedOn = ApplicationConfiguration.AreHierarchyIconsTurnedOn;
            EditorGUI.BeginChangeCheck();
            {
                areHierarchyIconsTurnedOn = EditorGUILayout.ToggleLeft(HierarchyIconsVisiblityToggleLabel, areHierarchyIconsTurnedOn);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ApplicationConfiguration.AreHierarchyIconsTurnedOn = areHierarchyIconsTurnedOn;
                Manager.UserSettings.Set(Constants.HierarchyIconsVisibilityToggleKey, areHierarchyIconsTurnedOn);
                EditorApplication.RepaintHierarchyWindow();
            }

            if (areHierarchyIconsTurnedOn)
            {
                areHierarchyIconsIndented = ApplicationConfiguration.AreHierarchyIconsIndented;
                EditorGUI.BeginChangeCheck();
                {
                    areHierarchyIconsIndented = EditorGUILayout.ToggleLeft(HierarchyIconsIndentToggleLabel, areHierarchyIconsIndented);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    ApplicationConfiguration.AreHierarchyIconsIndented = areHierarchyIconsIndented;
                    Manager.UserSettings.Set(Constants.HierarchyIconsIndentToggleKey, areHierarchyIconsIndented);
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
        }

        public override bool IsBusy => isBusy || userSettingsView.IsBusy || gitPathView.IsBusy;
    }
}
