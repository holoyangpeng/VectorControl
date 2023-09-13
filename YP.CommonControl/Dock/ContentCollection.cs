using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// define the collection to store the content
    /// </summary>
    public class ContentCollection:Common.CollectionWithEvents 
    {
        #region ..Constructor
        internal ContentCollection(Group group)
        {
            this._group = group;
        }
        #endregion

        #region ..private fields
        Group _group = null;
        #endregion

        #region ..public properties
        /// <summary>
        /// gets or sets the object as index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Content this[int index]
        {
            set
            {
                List[index] = value;
            }
            get
            {
                return List[index] as Content;
            }
        }
        #endregion

        #region ..Methods
        /// <summary>
        /// append the content to the end
        /// </summary>
        /// <param name="c"></param>
        public void Add(Content c)
        {
            if (!List.Contains(c))
                List.Add(c);
        }

        /// <summary>
        /// insert the content to the index
        /// </summary>
        /// <param name="c"></param>
        /// <param name="index"></param>
        public void Insert(Content c, int index)
        {
            index = (int)Math.Max(0, index);
            if (index > this.Count - 1)
                this.Add(c);
            else
                List.Insert(index, c);
        }

        /// <summary>
        /// remove the content from the collection
        /// </summary>
        /// <param name="c"></param>
        public void Remove(Content c)
        {
            int index = List.IndexOf(c);
            if (index >= 0)
                this.RemoveAt(index);
        }

        /// <summary>
        /// get the index of the content
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int IndexOf(Content c)
        {
            return List.IndexOf(c);
        }

        /// <summary>
        /// judge the content exists in the collection
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool Contains(Content c)
        {
            return List.Contains(c);
        }

        /// <summary>
        /// copy all the contents to the array
        /// </summary>
        /// <param name="contents"></param>
        public void CopyTo(Content[] contents)
        {
            this.CopyTo(contents, 0);
        }

        /// <summary>
        /// copy all the contents to the array
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="index"></param>
        public void CopyTo(Content[] contents, int index)
        {
            List.CopyTo(contents, index);
        }
        #endregion

        #region ..OnInsertComplete
        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);
            Content c = value as Content;
            if (c != null && this._group != null)
                c._parentGroup = this._group;
        }
        #endregion

        #region ..IEnumerator
        public new ContentEnumerator GetEnumerator()
        {
            return new ContentEnumerator(this);
        }

        public class ContentEnumerator : object, IEnumerator
        {

            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public ContentEnumerator(ContentCollection mappings)
            {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public Content Current
            {
                get
                {
                    return ((Content)(baseEnumerator.Current));
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
