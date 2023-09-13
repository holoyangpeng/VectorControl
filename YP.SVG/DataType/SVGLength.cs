using System;
using System.Text.RegularExpressions;

namespace YP.SVG.DataType
{
    /// <summary>
    /// ʵ��SVG�еĳ��ȶ���
    /// </summary>
    public struct SVGLength : Interface.DataType.ISVGLength
    {
        static Regex reLength = new Regex("^" + SVGNumber.NumberPattern + @"\s*(?<type>[a-z\%]+)?$");

        #region ..���켰����
        public SVGLength(string lengthstr, Interface.ISVGElement ownerElement, LengthDirection direction)
        {
            //
            // TODO: �ڴ˴���ӹ��캯���߼�
            //
            this.defaultValue = string.Empty;
            this.ownerElement = ownerElement;
            this.direction = direction;
            this.isEmpty = false;
            this.valuestr = lengthstr;
            this.floatvalue = 0;
            this.valueInSpecifiedUnits = 0;
            this.unitType = YP.SVG.LengthType.SVG_LENGTHTYPE_PX;
            this.ParseLength(lengthstr);

        }

        public SVGLength(string lengthstr, Interface.ISVGElement ownerElement, LengthDirection direction, string defaultValue)
        {
            //
            // TODO: �ڴ˴���ӹ��캯���߼�
            //
            this.defaultValue = defaultValue;
            this.ownerElement = ownerElement;
            this.direction = direction;
            this.isEmpty = false;
            if (lengthstr ==null|| lengthstr.Trim().Length == 0)
                lengthstr = this.defaultValue;
            this.valuestr = lengthstr;
            this.floatvalue = 0;
            this.valueInSpecifiedUnits = 0;
            this.unitType = YP.SVG.LengthType.SVG_LENGTHTYPE_PX;
            this.ParseLength(lengthstr);
            defaultValue = null;
            lengthstr = null;
        }

        public SVGLength(float lengthvalue, Interface.ISVGElement ownerElement, LengthDirection direction)
        {
            this.defaultValue = string.Empty;
            this.floatvalue = lengthvalue;
            this.valueInSpecifiedUnits = this.floatvalue;
            this.ownerElement = ownerElement;
            this.direction = direction;
            this.isEmpty = false;
            this.unitType = LengthType.SVG_LENGTHTYPE_PX;
            this.valuestr = lengthvalue.ToString();
        }

        public SVGLength(float lengthValue, LengthType type)
        {
            this.defaultValue = string.Empty;
            this.floatvalue = lengthValue;
            this.valueInSpecifiedUnits = this.floatvalue;
            this.ownerElement = null;
            this.direction = LengthDirection.Hori;
            this.isEmpty = false;
            this.unitType = type;
            this.valuestr = lengthValue.ToString();
        }

        public SVGLength(float lengthValue):this(lengthValue,  LengthType.SVG_LENGTHTYPE_PX)
        {
            
        }
        #endregion

        #region ..˽�б���
        LengthType unitType;
        float floatvalue;
        float valueInSpecifiedUnits;
        public Interface.ISVGElement ownerElement;
        public LengthDirection direction;
        string valuestr;
        bool isEmpty;
        string defaultValue;
        #endregion

        #region ..��̬����
        static SVGLength length = new SVGLength();

        /// <summary>
        /// �ж϶����Ƿ�Ϊ��
        /// </summary>
        public static SVGLength Empty
        {
            get
            {
                length.isEmpty = true;
                return length;
            }
        }
        #endregion

        #region ..��������
        /// <summary>
        /// ��ȡ�����ó��ȵ�λ
        /// </summary>
        public LengthType UnitType
        {
            get
            {
                return this.unitType;
            }
            set
            {
                this.unitType = value;
            }
        }

        /// <summary>
        /// ��ȡ�����Ĭ��ֵ
        /// </summary>
        public string DefaultValue
        {
            get
            {
                return this.defaultValue;
            }
        }

        /// <summary>
        /// �ж϶����Ƿ�Ϊ��
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.isEmpty;
            }
        }

        /// <summary>
        /// ��ȡ����˳��ȵ�ԭʼ�ַ���
        /// </summary>
        public string OriValueStr
        {
            get
            {
                return this.valuestr;
            }
        }

        /// <summary>
        /// ��ȡ�ø��������ĽǶ�ֵ�����������û����굥λ�����´����Խ�����ValueInSpecifiedUnits��ValueString����ͬ���ı�
        /// </summary>
        public float Value
        {
            get
            {
                float ret = 0;
                switch (UnitType)
                {
                    case LengthType.SVG_LENGTHTYPE_NUMBER:
                    case LengthType.SVG_LENGTHTYPE_PX:
                        ret = ValueInSpecifiedUnits;
                        break;
                    case LengthType.SVG_LENGTHTYPE_EMS:
                        // TODO: this should find the current font-size for the OwnerElement
                        ret = ValueInSpecifiedUnits * 10;
                        break;
                    case LengthType.SVG_LENGTHTYPE_EXS:
                        // TODO: this should find the current font-size for the OwnerElement
                        ret = ValueInSpecifiedUnits * 5F;
                        break;
                    case LengthType.SVG_LENGTHTYPE_CM:
                        ret = ValueInSpecifiedUnits * 35.43307F;
                        break;
                    case LengthType.SVG_LENGTHTYPE_MM:
                        ret = ValueInSpecifiedUnits * 3.543307F;
                        break;
                    case LengthType.SVG_LENGTHTYPE_IN:
                        ret = ValueInSpecifiedUnits * 90;
                        break;
                    case LengthType.SVG_LENGTHTYPE_PT:
                        ret = ValueInSpecifiedUnits * 1.25F;
                        break;
                    case LengthType.SVG_LENGTHTYPE_PC:
                        ret = ValueInSpecifiedUnits * 15;
                        break;
                    case LengthType.SVG_LENGTHTYPE_PERCENTAGE:
                        if (this.ownerElement is Interface.GradientsAndPatterns.ISVGGradientElement)
                        {
                            ret = ValueInSpecifiedUnits / 100F;
                        }
                        else if (this.ownerElement != null)
                        {
                            //if ownerElement is textblock
                            Interface.ISVGTextBlockContainer svp = ((this.ownerElement.ViewPortElement != null) ? this.ownerElement.ViewPortElement : this.ownerElement) as Interface.ISVGTextBlockContainer;

                            if (svp != null)
                            {
                                if (this.direction == LengthDirection.Hori)
                                {
                                    ret = ValueInSpecifiedUnits * svp.Viewport.Bounds.Width / 100;
                                }
                                else if (direction == LengthDirection.Vect)
                                {
                                    ret = ValueInSpecifiedUnits * svp.Viewport.Bounds.Height / 100;
                                }
                                else
                                {
                                    float actualWidth = svp.Viewport.Bounds.Width;
                                    float actualHeight = svp.Viewport.Bounds.Height;
                                    ret = Convert.ToSingle(Math.Sqrt(actualWidth * actualWidth + actualHeight * actualHeight) / Math.Sqrt(2)) * ValueInSpecifiedUnits / 100;
                                }
                            }
                        }
                        else
                            ret = ValueInSpecifiedUnits / 100F;
                        break;
                    //					case LengthType.SVG_LENGTHTYPE_UNKNOWN:
                    //						throw new SvgException(SvgExceptionType.SVG_INVALID_VALUE_ERR, "Bad length unit");
                }
                return ret;
            }
            set
            {
                this.valueInSpecifiedUnits = value;
            }
        }

        /// <summary>
        /// ��ȡ�ø��������ĽǶ�ֵ���������Ͳ��ñ����ĳ�������
        /// </summary>
        public float ValueInSpecifiedUnits
        {
            get
            {
                return this.valueInSpecifiedUnits;
            }
        }

        /// <summary>
        /// ��ȡ��ʾ����ֵ���ַ���
        /// </summary>
        public string ValueAsString
        {
            get
            {
                return ValueInSpecifiedUnits + this.TypeString;
            }
        }

        /// <summary>
        /// ��ȡ�����ַ���
        /// </summary>
        public string TypeString
        {
            get
            {
                string unit = "";
                switch (UnitType)
                {
                    case LengthType.SVG_LENGTHTYPE_PERCENTAGE:
                        unit = "%";
                        break;
                    case LengthType.SVG_LENGTHTYPE_EMS:
                        unit = "em";
                        break;
                    case LengthType.SVG_LENGTHTYPE_EXS:
                        unit = "ex";
                        break;
                    case LengthType.SVG_LENGTHTYPE_PX:
                        //						unit = "px";
                        break;
                    case LengthType.SVG_LENGTHTYPE_CM:
                        unit = "cm";
                        break;
                    case LengthType.SVG_LENGTHTYPE_MM:
                        unit = "mm";
                        break;
                    case LengthType.SVG_LENGTHTYPE_IN:
                        unit = "in";
                        break;
                    case LengthType.SVG_LENGTHTYPE_PT:
                        unit = "pt";
                        break;
                    case LengthType.SVG_LENGTHTYPE_PC:
                        unit = "pc";
                        break;
                }
                return unit;
            }
        }
        #endregion

        #region ..���賤��ֵ����ָ���ĳ������ͺ�ָ����ֵ
        /// <summary>
        /// ���賤��ֵ����ָ���ĳ������ͺ�ָ����ֵ
        /// </summary>
        /// <param name="unitType">��������</param>
        /// <param name="angleValue">����������ʾ����ֵ</param>
        public void NewValueSpecifiedUnits(LengthType unitType, float angleValue)
        {
            this.unitType = unitType;
            this.valueInSpecifiedUnits = angleValue;
        }
        #endregion

        #region ..������ת��Ϊ�ض��ĳ�������
        /// <summary>
        /// ������ת��Ϊ�ض��ĳ�������
        /// </summary>
        /// <param name="unitType">��ת���ĳ�������</param>
        public void ConvertToSpecifiedUnits(LengthType unitType)
        {
            this.unitType = unitType;
        }
        #endregion

        #region ..��������
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="lengthstr">�����ַ���</param>
        void ParseLength(string lengthstr)
        {
            string s = lengthstr;
            s = s.Trim();
            if (string.Compare(s,"")==0)
            {
                NewValueSpecifiedUnits(LengthType.SVG_LENGTHTYPE_NUMBER, 0);
            }
            else
            {
                Match match = reLength.Match(s);
                if (match.Success)
                {
                    LengthType unitType;
                    float length = SVGNumber.ParseNumberStr(match.Groups["number"].Value);

                    if (match.Groups["type"].Success)
                    {
                        switch (match.Groups["type"].Value)
                        {
                            case "%":
                                unitType = LengthType.SVG_LENGTHTYPE_PERCENTAGE;
                                break;
                            case "em":
                                unitType = LengthType.SVG_LENGTHTYPE_EMS;
                                break;
                            case "ex":
                                unitType = LengthType.SVG_LENGTHTYPE_EXS;
                                break;
                            case "px":
                                unitType = LengthType.SVG_LENGTHTYPE_PX;
                                break;
                            case "cm":
                                unitType = LengthType.SVG_LENGTHTYPE_CM;
                                break;
                            case "mm":
                                unitType = LengthType.SVG_LENGTHTYPE_MM;
                                break;
                            case "in":
                                unitType = LengthType.SVG_LENGTHTYPE_IN;
                                break;
                            case "pt":
                                unitType = LengthType.SVG_LENGTHTYPE_PT;
                                break;
                            case "pc":
                                unitType = LengthType.SVG_LENGTHTYPE_PC;
                                break;
                            default:
                                unitType = LengthType.SVG_LENGTHTYPE_UNKNOWN;
                                break;
                        }
                    }
                    else
                    {
                        unitType = LengthType.SVG_LENGTHTYPE_NUMBER;
                    }
                    NewValueSpecifiedUnits(unitType, length);
                }
                else
                {
                    //					throw new SvgException(SvgExceptionType.SVG_INVALID_VALUE_ERR, "Bad length format : " + s);
                }
            }
        }
        #endregion

        #region ..��ȡ����ֵ���ı����
        /// <summary>
        /// ��ȡ����ֵ���ı����
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ValueAsString;
        }
        #endregion

        #region ..float value ��SVGLength����ʽת��
        public static implicit operator SVGLength(float x)
        {
            return new SVGLength(x);
        }
        #endregion
    }
}
