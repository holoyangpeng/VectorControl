using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections.Specialized;
using System.Collections;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// define the collection to store the group
    /// </summary>
    public class GroupCollection:Common.CollectionWithEvents
    {
        #region ..Constructor
        internal GroupCollection()
        {

        }
        #endregion

        #region ..private fields
        //indicates whether the collection display the control
        bool _displayControl = true;

        //the size to group
        Hashtable _groupSizes = new Hashtable();
        #endregion

        #region ..public properties
        /// <summary>
        /// sets a value indicates whether the group displays when exist in this collection
        /// </summary>
        internal bool DisplayControl
        {
            set
            {
                this._displayControl = value;
            }
        }

        /// <summary>
        /// gets or sets the object as index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Group this[int index]
        {
            set
            {
                List[index] = value;
            }
            get
            {
                return List[index] as Group;
            }
        }
        #endregion

        #region ..Methods
        /// <summary>
        /// append the Group to the end
        /// </summary>
        /// <param name="c">the group you want to add</param>
        public void Add(Group c)
        {
            if (!List.Contains(c))
                List.Add(c);
        }

        /// <summary>
        /// insert the Group to the index
        /// </summary>
        /// <param name="c">the group you want to add</param>
        /// <param name="index"></param>
        public void Insert(Group c, int index)
        {
            index = (int)Math.Max(0, index);
            if (index > this.Count - 1)
                this.Add(c);
            else
                List.Insert(index, c);
        }

        /// <summary>
        /// remove the Group from the collection
        /// </summary>
        /// <param name="c"></param>
        public void Remove(Group c)
        {
            int index = List.IndexOf(c);
            if (index >= 0)
                this.RemoveAt(index);
        }

        /// <summary>
        /// get the index of the Group
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int IndexOf(Group c)
        {
            return List.IndexOf(c);
        }

        /// <summary>
        /// judge the Group exists in the collection
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool Contains(Group c)
        {
            return List.Contains(c);
        }

        /// <summary>
        /// copy all the contents to the array
        /// </summary>
        /// <param name="contents"></param>
        public void CopyTo(Group[] contents)
        {
            this.CopyTo(contents, 0);
        }

        /// <summary>
        /// copy all the contents to the array
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="index"></param>
        public void CopyTo(Group[] contents, int index)
        {
            List.CopyTo(contents, index);
        }
        #endregion

        #region ..override
        protected override void OnInsert(int index, object value)
        {
            Group group = value as Group;
            if (group != null && group.ParentGroups != null && group.ParentGroups._displayControl)
                group.ParentGroups.Remove(group);
            base.OnInsert(index, value);
            if (group != null)
                group.ParentGroups = this;
        }
        #endregion

        #region ..IEnumerator
        public new GroupEnumerator GetEnumerator()
        {
            return new GroupEnumerator(this);
        }

        public class GroupEnumerator : object, IEnumerator
        {

            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public GroupEnumerator(GroupCollection mappings)
            {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public Group Current
            {
                get
                {
                    return ((Group)(baseEnumerator.Current));
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return baseEnumerator.Current;
                }
            }

            public bool MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            bool IEnumerator.MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            public void Reset()
            {
                baseEnumerator.Reset();
            }

            void IEnumerator.Reset()
            {
                baseEnumerator.Reset();
            }
        }
        #endregion
    }
}
