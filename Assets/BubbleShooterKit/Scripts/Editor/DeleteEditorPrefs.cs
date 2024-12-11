// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEditor;

namespace BubbleShooterKit
{
    /// <summary>
    /// Utility class for deleting the EditorPrefs from within a menu option.
    /// </summary>
    public class DeleteEditorPrefs
    {
        [MenuItem("Tools/Bubble Shooter Kit/Delete EditorPrefs", false, 2)]
        public static void DeleteAllEditorPrefs()
        {
            EditorPrefs.DeleteAll();
        }
    }
}
