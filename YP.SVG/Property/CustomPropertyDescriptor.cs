using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

namespace YP.SVG.Property
{
	/// <summary>
	/// define the descriptor for the custom property
	/// </summary>
    internal class CustomPropertyDescriptor : PropertyDescriptor
	{
		#region ..Constructor
		public CustomPropertyDescriptor(IProperty item, YP.SVG.SVGElement bag, string name, Attribute[] attrs) :
			base(name, attrs)
		{
			this.bag = bag;
			this.item = item;
		}
		#endregion

		#region ..private fields
		private YP.SVG.SVGElement bag;
		private IProperty item;
		#endregion

		#region ..override base methods
		public override Type ComponentType
		{
			get { return item.GetType(); }
		}

		public override bool IsReadOnly
		{
			get { return (Attributes.Matches(ReadOnlyAttribute.Yes)); }
		}

		public override Type PropertyType
		{
			get { return item.PropertyType; }
		}

		public override bool CanResetValue(object component)
		{
			if(item.DefaultValue == null)
				return false;
			else
				return !this.GetValue(component).Equals(item.DefaultValue);
		}

		public override object GetValue(object component)
		{
			//return this.bag.GetPropertyValue(this.item.AttributeName);
			if(this.item is CustomProperty)
				return (this.item as CustomProperty).GetPropertyValue();
			return null;
		}

		public override void ResetValue(object component)
		{
			SetValue(component, item.DefaultValue);
		}

		public override void SetValue(object component, object value)
		{
			if(this.item is CustomProperty)
				(this.item as CustomProperty).SetPropertyValue(value);
		}

		public override bool ShouldSerializeValue(object component)
		{
			object val = this.GetValue(component);

			if(item.DefaultValue == null && val == null)
				return false;
			else
				return !val.Equals(item.DefaultValue);
		}
		#endregion
	}
}
