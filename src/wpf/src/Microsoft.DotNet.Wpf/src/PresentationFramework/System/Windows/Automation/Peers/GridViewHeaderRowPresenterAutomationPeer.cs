﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// 
    public class GridViewHeaderRowPresenterAutomationPeer : FrameworkElementAutomationPeer
    {
        ///
        public GridViewHeaderRowPresenterAutomationPeer(GridViewHeaderRowPresenter owner)
            : base(owner)
        {
        }

        ///
        protected override string GetClassNameCore()
        {
            return "GridViewHeaderRowPresenter";
        }

        ///
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Header;
        }

        // AutomationControlType.Header must return IsContentElement false.
        // See http://msdn.microsoft.com/en-us/library/ms753110.aspx
        protected override bool IsContentElementCore()
        {
            return false;
        }

        ///
        protected override List<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> list = base.GetChildrenCore();
            List<AutomationPeer> newList = null;
            if (list != null) 
            {
                newList = new List<AutomationPeer>(list.Count);
                //GVHRP contains 2 extra column headers, one is dummy header, the other is floating header
                //We need to remove them from the tree
                foreach (AutomationPeer peer in list)
                {
                    if (peer is UIElementAutomationPeer)
                    {
                        GridViewColumnHeader header = ((UIElementAutomationPeer)peer).Owner as GridViewColumnHeader;
                        if (header != null && header.Role == GridViewColumnHeaderRole.Normal)
                        {
                            //Because GVHRP uses inverse sequence to store column headers, we need to use insert here
                            newList.Insert(0, peer);
                        }
                    }
                }
            }
            return newList;
        }
    }
}
