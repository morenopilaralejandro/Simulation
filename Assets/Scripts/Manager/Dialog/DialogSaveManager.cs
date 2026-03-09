using UnityEngine;
using System.Collections.Generic;

    /// <summary>
    /// Handles saving and loading ink story state.
    /// Integrate this with your main save system.
    /// </summary>
    public class DialogSaveManager : MonoBehaviour
    {
        /*
        [SerializeField] private InkStoryManager _storyManager;
        
        private HashSet<string> _viewedDialogs = new HashSet<string>();

        /// <summary>
        /// Mark a dialog as viewed (for "new dialog" indicators, etc.)
        /// </summary>
        public void MarkDialogViewed(string dialogId)
        {
            _viewedDialogs.Add(dialogId);
        }

        public bool HasViewedDialog(string dialogId)
        {
            return _viewedDialogs.Contains(dialogId);
        }

        /// <summary>
        /// Gather all dialog state into a serializable object.
        /// Call this from your main save system.
        /// </summary>
        public DialogSaveData GetSaveData()
        {
            var saveData = new DialogSaveData();
            
            // Save each story's state
            string[] storyIds = { "main", "side_stories", "tutorials" }; // your story IDs
            foreach (var id in storyIds)
            {
                var stateJson = _storyManager.GetStoryState(id);
                if (!string.IsNullOrEmpty(stateJson))
                {
                    saveData.storyStates.Add(new DialogSaveData.StorySaveEntry
                    {
                        storyId = id,
                        stateJson = stateJson
                    });
                }
            }
            
            saveData.viewedDialogIds = new List<string>(_viewedDialogs);
            return saveData;
        }

        /// <summary>
        /// Restore dialog state from save data.
        /// Call this from your main save system after loading.
        /// </summary>
        public void LoadSaveData(DialogSaveData saveData)
        {
            if (saveData == null) return;
            
            foreach (var entry in saveData.storyStates)
            {
                _storyManager.LoadStoryState(entry.storyId, entry.stateJson);
            }
            
            _viewedDialogs = new HashSet<string>(saveData.viewedDialogIds);
        }
*/
    }
