using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// define the collection to store the zone
    /// </summary>
    public class ZoneCollection : Common.CollectionWithEvents
    {
        #region ..public properties
        /// <summary>
        /// gets or sets the object as index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Zone this[int index]
        {
            set
            {
                List[index] = value;
            }
            get
            {
                return List[index] as Zone;
            }
        }
        #endregion

        #region ..Methods
        /// <summary>
        /// append the Zone to the end
        /// </summary>
        /// <param name="c">the group you want to add</param>
        public void Add(Zone c)
        {
            if (!List.Contains(c))
                List.Add(c);
        }

        /// <summary>
        /// insert the Zone to the index
        /// </summary>
        /// <param name="c">the group you want to add</param>
        /// <param name="displaySize">the initial size of the group when it expand</param>
        /// <param name="index"></param>
        public void Insert(Zone c, int index)
        {
            index = (int)Math.Max(0, index);
            if (index > this.Count - 1)
                this.Add(c);
            else
                List.Insert(index, c);
        }

        /// <summary>
        /// remove the Zone from the collection
        /// </summary>
        /// <param name="c"></param>
        public void Remove(Zone c)
        {
            int index = List.IndexOf(c);
            if (index >= 0)
                this.RemoveAt(index);
        }

        /// <summary>
        /// get the index of the Zone
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int IndexOf(Zone c)
        {
            return List.IndexOf(c);
        }

        /// <summary>
        /// judge the Zone exists in the collection
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool Contains(Zone c)
        {
            return List.Contains(c);
        }

        /// <summary>
        /// copy all the contents to the array
        /// </summary>
        /// <param name="contents"></param>
        public void CopyTo(Zone[] contents)
        {
            this.CopyTo(contents, 0);
        }

        /// <summary>
        /// copy all the contents to the array
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="index"></param>
        public void CopyTo(Zone[] contents, int index)
        {
            List.CopyTo(contents, index);
        }
        #endregion

        #region ..Clone
        /// <summary>
        /// create the back instance for the collection
        /// </summary>
        /// <returns></returns>
        public ZoneCollection Clone()
        {
            ZoneCollection zones = new ZoneCollection();
            foreach (Zone zone in this)
                zones.Add(zone);
            return null;
        }
        #endregion

        #region ..IEnumerator
        public new ZoneEnumerator GetEnumerator()
        {
            return new ZoneEnumerator(this);
        }

        public class ZoneEnumerator : object, IEnumerator
        {

            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public ZoneEnumerator(ZoneCollection mappings)
            {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public Zone Current
            {
                get
                {
                    return ((Zone)(baseEnumerator.Current));
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
