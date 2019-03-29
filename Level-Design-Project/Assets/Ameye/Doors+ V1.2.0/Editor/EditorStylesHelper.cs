// EditorStylesHelper.cs
// Created by Alexander Ameye
// Version 1.1.0

using UnityEngine;
using UnityEditor;

namespace StylesHelper
{
    public static class Styles
    {
        internal static GUIStyle PublisherNameStyle;
        internal static GUIStyle ToolBarStyle;
        internal static GUIStyle GreyText;
        internal static GUIStyle ReviewBanner;

        internal static GUIContent PublisherNameContent;
        internal static GUIContent support;
        internal static GUIContent contact;
        internal static GUIContent review;


        internal static GUIStyle helpbox;
        internal static GUIStyle noErrors;
        internal static GUIStyle centeredVersionLabel;
        internal static GUIStyle helpIcon;
        internal static GUIStyle previewIcon;

     
        internal static GUIContent HelpIcon;
        internal static GUIContent NewIcon;
        internal static GUIContent VersionLabel;
        internal static GUIContent errordetection;

        internal static GUIContent previewButton;

        // Error Detection
        internal static GUIContent PlayerTagTrue;
        internal static GUIContent PlayerTagFalse;

        internal static GUIContent DetectionTrue;
        internal static GUIContent DetectionFalse;
        internal static GUIContent DetectionUnknown;

        internal static GUIContent ReachTrue;
        internal static GUIContent ReachFalse;
        internal static GUIContent ReachUnknown;

        internal static GUIContent TagTrue;
        internal static GUIContent TagFalse;

        internal static GUIContent LayerTrue;
        internal static GUIContent LayerFalse;



        static Styles()
        {
            PublisherNameContent = IconContent("<size=20><b><color=#666666>Alex</color></b></size>", "", "");

            PublisherNameStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };

            ToolBarStyle = new GUIStyle("LargeButtonMid")
            {
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };

            GreyText = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleLeft
            };

            ReviewBanner = new GUIStyle((GUIStyle)"TL SelectionButton")
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };

            support = IconContent("<size=11><b> Support</b></size>\n <size=11>Get help.\n</size>", "support", "");
            contact = IconContent("<size=11><b> Contact</b></size>\n <size=11>Reach out.\n</size>", "contact", "");
            review = IconContent("<size=11> Please consider leaving us a review.</size>", "award_star_gold_blue", "");



           
            previewButton = IconContent("", "preview", "Preview the sound.");
           // previewButton = EditorGUIUtility.IconContent("PlayButton", "Preview the sound.");

            NewIcon = IconContent(" Whats new?", "new", "");
            errordetection = IconContent(" Detect errors!", "errordetection", "");
            VersionLabel = IconContent("Version 1.2.0", "", "");
            HelpIcon = IconContent("", "help_large", "Need help?");

            PlayerTagTrue = IconContent(" Player", "true", "");
            PlayerTagFalse = IconContent(" Player", "false", "");

            DetectionTrue = IconContent(" Detection", "true", "");
            DetectionFalse = IconContent(" Detection", "false", "");
            DetectionUnknown = IconContent(" Detection", "help", "");

            ReachTrue = IconContent(" Reach", "true", "");
            ReachFalse = IconContent(" Reach", "false", "");
            ReachUnknown = IconContent(" Reach", "help", "");

            TagTrue = IconContent(" Door", "true", "");
            TagFalse = IconContent(" Door", "false", "");

            LayerTrue = IconContent(" Trigger Zones", "true", "");
            LayerFalse = IconContent(" Trigger Zones", "false", "");

            helpbox = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleLeft,
                richText = true
         
            };

            noErrors = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true

            };
            

            centeredVersionLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)

            };

            helpIcon = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleRight,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };

            previewIcon = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(1, 1, 1, 1),
                margin = new RectOffset(0, 0, 0, 0)
            };

            previewIcon.contentOffset = new Vector2(0, 3);


        }

        static GUIContent IconContent(string text, string icon, string tooltip)
        {
            Texture2D cached = (Texture2D)Resources.Load("Icons/" + icon);  
            return new GUIContent(text, cached, tooltip);
        }
    }
}
