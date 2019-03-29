using UnityEditor;
using UnityEngine;

namespace PublisherSupportWindow
{
    public class SupportWindow : EditorWindow
    {
        private GUILayoutOption _bannerHeight;
        private GUIStyle _centeredVersionLabel;
        private GUIStyle _greyText;
        private GUIStyle _publisherNameStyle;
        private GUIStyle _reviewBanner;

        private bool _stylesNotLoaded = true;
        private GUILayoutOption _toolbarHeight;
        private int _toolBarIndex;
        private GUIContent[] _toolbarOptions;
        private GUIStyle _toolBarStyle;

        [MenuItem("Tools/Doors+/About")]
        private static void ShowWindow()
        {
            var myWindow = GetWindow<SupportWindow>("About");
            myWindow.minSize = new Vector2(300, 400);
            myWindow.maxSize = myWindow.minSize;
            myWindow.titleContent = new GUIContent("About");
            myWindow.Show();
        }

        private void OnEnable()
        {
            _toolbarOptions = new GUIContent[2];
            _toolbarOptions[0] =
                new GUIContent("<size=11><b> Support</b></size>\n <size=11>Get help and talk \n with others.</size>",
                    (Texture2D) Resources.Load("Icons/support"), "");  
            _toolbarOptions[1] =
                new GUIContent("<size=11><b> Contact</b></size>\n <size=11>Reach out and \n get help.</size>",
                    (Texture2D)Resources.Load("Icons/contact"), "");
            _toolbarHeight = GUILayout.Height(50);
            _bannerHeight = GUILayout.Height(30);
        }

        private void LoadStyles()
        {
            _publisherNameStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };

            _toolBarStyle = new GUIStyle("LargeButtonMid")
            {
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };

            _greyText = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleLeft
            };

            _centeredVersionLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };

            _reviewBanner = new GUIStyle("TL SelectionButton")
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };

            _stylesNotLoaded = false;
        }

        private void OnGUI()
        {
            if (_stylesNotLoaded) LoadStyles();

                EditorGUILayout.Space();
                GUILayout.Label(new GUIContent("<size=20><b><color=#666666>Alex</color></b></size>"),
                    _publisherNameStyle);
                EditorGUILayout.Space();

                _toolBarIndex = GUILayout.Toolbar(_toolBarIndex, _toolbarOptions, _toolBarStyle, _toolbarHeight);

                switch (_toolBarIndex)
                {
                    case 0:
                        EditorGUILayout.Space();
                        if (GUILayout.Button("Support Forum", EditorStyles.label))
                            Application.OpenURL(
                                "https://forum.unity3d.com/threads/wip-doors-pro-a-powerful-door-system.459866/");
                        EditorGUILayout.LabelField("Talk with others.", _greyText);

                        EditorGUILayout.Space();
                        if (GUILayout.Button("Documentation", EditorStyles.label))
                            Application.OpenURL("https://alexdoorsystem.github.io/");
                        EditorGUILayout.LabelField("Detailed documentation and quick-start guides.", _greyText);

                        EditorGUILayout.Space();
                        if (GUILayout.Button("YouTube Tutorials", EditorStyles.label))
                            Application.OpenURL(
                                "https://www.youtube.com/channel/UC2un52brwq6pax6fwUeqL8w?view_as=subscriber");
                        EditorGUILayout.LabelField("Easy-to-digest tutorial videos and showcases.", _greyText);

                        EditorGUILayout.Space();
                        if (GUILayout.Button("Changelog", EditorStyles.label))
                            Application.OpenURL("https://trello.com/b/hsUpSAoq/doors-v120");
                        EditorGUILayout.LabelField("See what the latest new features are.", _greyText);
                        break;

                    case 1:
                        EditorGUILayout.Space();
                        if (GUILayout.Button("Email", EditorStyles.label))
                            Application.OpenURL("mailto:alexanderameye@gmail.com?");
                        EditorGUILayout.LabelField("Get in touch with me.", _greyText);

                        EditorGUILayout.Space();
                        if (GUILayout.Button("Twitter", EditorStyles.label))
                            Application.OpenURL("https://twitter.com/alexanderameye");
                        EditorGUILayout.LabelField("See what I'm working on.", _greyText);
                        break;
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(new GUIContent("Version 1.2.0"), _centeredVersionLabel);
                EditorGUILayout.Space();
                if (GUILayout.Button(
                    new GUIContent("<size=11> Please consider leaving us a review.</size>",
                        (Texture2D) Resources.Load("Icons/award_star_gold_blue"), ""), _reviewBanner,
                    _bannerHeight))
                    Application.OpenURL(
                        "https://www.assetstore.unity3d.com/en/#!/account/downloads/search=Doors");
        }

    }
}