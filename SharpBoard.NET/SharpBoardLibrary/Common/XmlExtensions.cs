//    Copyright 2010 SharpBoard Library authors
//
//    This file is part of SharpBoard Library.
//
//    SharpBoard Library is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    SharpBoard Library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with SharpBoard Library.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharpBoardLibrary
{
    public static class XmlExtensions
    {
            public static string GetAttribute(this XElement element, string attribute)
            {
                return element.GetAttribute(attribute, true, null);
            }

            public static string CaseAttribute(this XElement element, string attribute)
            {
                return element.CaseAttribute(attribute, true,null);
            }

            public static string GetAttribute(this XElement element, string attribute, bool opzional, string def)
            {
                XAttribute a = element.Attribute(attribute);
                if (a == null)
                    if (!opzional)
                        throw new UnknowAttributeException(element.Name.ToString(), attribute);
                    else
                        return def;
                return a.Value.Trim();
            }

            public static string CaseAttribute(this XElement element, string attribute, bool opzional, string def)
            {
                XAttribute a = element.Attribute(attribute);
                if (a == null)
                    if (!opzional)
                        throw new UnknowAttributeException(element.Name.ToString(), attribute);
                    else
                        return def;
                return a.Value.Trim();
            }

            public static int GetIntAttribute(this XElement element, string attribute)
            {
                return element.GetIntAttribute(attribute, false, 0);
            }

            public static int GetIntAttribute(this XElement element, string attribute, bool opzional, int def)
            {
                XAttribute a = element.Attribute(attribute);
                if (a == null)
                    if (!opzional)
                        throw new UnknowAttributeException(element.Name.ToString(), attribute);
                    else
                        return def;
                try
                {
                    string s = a.Value.Trim();
                    int tmp = int.Parse(s);
                    return tmp;
                }
                catch
                {
                    throw new InvalidAttributeException(element.Name.ToString(), attribute);
                }
            }

            public static bool GetBoolAttribute(this XElement element, string attribute)
            {
                return element.GetBoolAttribute(attribute, false, false);
            }

            public static bool GetBoolAttribute(this XElement element, string attribute, bool opzional, bool defaultValue)
            {
                XAttribute a = element.Attribute(attribute);
                if (a == null)
                    if (!opzional)
                        throw new UnknowAttributeException(element.Name.ToString(), attribute);
                    else
                        return defaultValue;

                string s = a.Value.Trim().ToLower();
                if (s != "true" && s != "false")
                    throw new InvalidAttributeException(element.Name.ToString(), attribute);
                return s == "true";
            }

            public static void AssertTagException(XElement element, string tag)
            {
                if (element == null)
                    throw new TagException(tag);
            }
    }

    #region XmlExceptions
    public class TagException : ApplicationException
    {
        protected string tag;
        public TagException(string tag)
        {
            this.tag = tag;
        }

        public override string Message
        {
            get { return string.Format("Tag \"{0}\" not found", tag); }
        }
    }

    public class InvalidTagException : TagException
    {
        public InvalidTagException(string tag)
            : base(tag)
        { }

        public override string Message
        {
            get { return string.Format("Invalid Tag \"{0}\"", tag); }
        }
    }

    public class AttributeException : ApplicationException
    {
        protected string attribute;
        protected string tag;
        public AttributeException(string tag, string attribute)
        {
            this.tag = tag;
            this.attribute = attribute;
        }
    }

    public class UnknowAttributeException : AttributeException
    {
        public UnknowAttributeException(string tag, string attribute) :
            base(tag, attribute)
        { }

        public override string Message
        {
            get { return string.Format("Attribute \"{0}\" not found in tag \"{1}\"", attribute, tag); }
        }
    }

    public class InvalidAttributeException : AttributeException
    {
        public InvalidAttributeException(string tag, string attribute) :
            base(tag, attribute)
        { }

        public override string Message
        {
            get { return string.Format("Invalid Attribute \"{0}\" in tag \"{1}\"", attribute, tag); }
        }
    }
    #endregion XmlExceptions

}
