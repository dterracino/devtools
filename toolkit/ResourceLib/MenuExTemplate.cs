﻿//-----------------------------------------------------------------------
// <copyright company="CoApp Project">
//     ResourceLib Original Code from http://resourcelib.codeplex.com
//     Original Copyright (c) 2008-2009 Vestris Inc.
//     Changes Copyright (c) 2011 Garrett Serack . All rights reserved.
// </copyright>
// <license>
// MIT License
// You may freely use and distribute this software under the terms of the following license agreement.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
// the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE
// </license>
//-----------------------------------------------------------------------

namespace CoApp.Developer.Toolkit.ResourceLib {
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    ///   Extended menu template.
    /// </summary>
    public class MenuExTemplate : MenuTemplateBase {
        private CoApp.Toolkit.Win32.MenuExTemplate _header;
        private MenuExTemplateItemCollection _menuItems = new MenuExTemplateItemCollection();

        /// <summary>
        ///   Menu items.
        /// </summary>
        public MenuExTemplateItemCollection MenuItems {
            get { return _menuItems; }
            set { _menuItems = value; }
        }

        /// <summary>
        ///   Read the menu template.
        /// </summary>
        /// <param name = "lpRes">Address in memory.</param>
        internal override IntPtr Read(IntPtr lpRes) {
            _header = (CoApp.Toolkit.Win32.MenuExTemplate) Marshal.PtrToStructure(lpRes, typeof (CoApp.Toolkit.Win32.MenuExTemplate));

            var lpMenuItem = ResourceUtil.Align(lpRes.ToInt32() + Marshal.SizeOf(_header) + _header.wOffset);

            return _menuItems.Read(lpMenuItem);
        }

        /// <summary>
        ///   Write the menu template.
        /// </summary>
        /// <param name = "w">Binary stream.</param>
        internal override void Write(BinaryWriter w) {
            var head = w.BaseStream.Position;
            // write header
            w.Write(_header.wVersion);
            w.Write(_header.wOffset);
            // w.Write(_header.dwHelpId);
            // pad to match the offset value
            ResourceUtil.Pad(w, (UInt16) (_header.wOffset - 4));
            // seek to the beginning of the menu item per offset value
            // this may be behind, ie. the help id structure is part of the first popup menu
            w.BaseStream.Seek(head + _header.wOffset + 4, SeekOrigin.Begin);
            // write menu items
            _menuItems.Write(w);
        }

        /// <summary>
        ///   String representation of the menu in the MENUEX format.
        /// </summary>
        /// <returns>String representation of the menu.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine("MENUEX");
            sb.Append(_menuItems.ToString());
            return sb.ToString();
        }
    }
}