using System;
using System.Collections;

namespace YP.CommonControl.Common
{
    /// <summary>
    /// define the base class for the collection which invoke the events when the collection is changed
    /// </summary>
	public class CollectionWithEvents : CollectionBase
    {
        #region ..delegate
        public delegate void ClearEventHandler();
        public delegate void CollectionEventHandler(int index, object value);
        #endregion

        #region ..events
        // Collection change events
        public event ClearEventHandler Clearing;
        public event ClearEventHandler Cleared;
        public event CollectionEventHandler Inserting;
        public event CollectionEventHandler Inserted;
        public event CollectionEventHandler Removing;
        public event CollectionEventHandler Removed;
	
		// Overrides for generating events
		protected override void OnClear()
		{
			// Any attached event handlers?
			if (Clearing != null)
				Clearing();
		}	

		protected override void OnClearComplete()
		{
			// Any attached event handlers?
			if (Cleared != null)
				Cleared();
		}	

		protected override void OnInsert(int index, object value)
		{
			// Any attached event handlers?
			if (Inserting != null)
				Inserting(index, value);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			// Any attached event handlers?
			if (Inserted != null)
				Inserted(index, value);
		}

		protected override void OnRemove(int index, object value)
		{
			// Any attached event handlers?
			if (Removing != null)
				Removing(index, value);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			// Any attached event handlers?
			if (Removed != null)
				Removed(index, value);
		}

		public int IndexOf(object value)
		{
			// Find the 0 based index of the requested entry
			return base.List.IndexOf(value);
        }
        #endregion
    }
}
