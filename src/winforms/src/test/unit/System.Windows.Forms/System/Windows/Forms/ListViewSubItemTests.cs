﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ListViewSubItemTests
{
    [Fact]
    public void ListViewSubItem_Ctor_Default()
    {
        var subItem = new ListViewItem.ListViewSubItem();
        Assert.Equal(SystemColors.Window, subItem.BackColor);
        Assert.Equal(Rectangle.Empty, subItem.Bounds);
        Assert.Equal(Control.DefaultFont, subItem.Font);
        Assert.Equal(SystemColors.WindowText, subItem.ForeColor);
        Assert.Empty(subItem.Name);
        Assert.Null(subItem.Tag);
        Assert.Empty(subItem.Text);
    }

    public static IEnumerable<object[]> Ctor_ListViewItem_String_TestData()
    {
        yield return new object[] { null, null, string.Empty };
        yield return new object[] { new ListViewItem(), string.Empty, string.Empty };
        yield return new object[] { new ListViewItem(), "reasonable", "reasonable" };
        yield return new object[] { new ListViewItem() { BackColor = Color.Yellow, ForeColor = Color.Yellow, Font = SystemFonts.StatusFont }, "reasonable", "reasonable" };

        ListViewItem item = new();
        Assert.Null(item.ListView);
        yield return new object[] { item, "reasonable", "reasonable" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_ListViewItem_String_TestData))]
    public void ListViewSubItem_Ctor_ListViewItem_String(ListViewItem owner, string text, string expectedText)
    {
        var subItem = new ListViewItem.ListViewSubItem(owner, text);
        Assert.Equal(SystemColors.Window, subItem.BackColor);
        Assert.Equal(Rectangle.Empty, subItem.Bounds);
        Assert.Equal(Control.DefaultFont, subItem.Font);
        Assert.Equal(SystemColors.WindowText, subItem.ForeColor);
        Assert.Empty(subItem.Name);
        Assert.Null(subItem.Tag);
        Assert.Equal(expectedText, subItem.Text);
    }

    public static IEnumerable<object[]> Ctor_ListViewItem_String_Color_Color_Font_TestData()
    {
        yield return new object[] { null, null, Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { new ListViewItem(), string.Empty, Color.Red, Color.Blue, SystemFonts.MenuFont, Color.Red, Color.Blue, string.Empty };
        yield return new object[] { new ListViewItem(), "reasonable", Color.Red, Color.Blue, SystemFonts.MenuFont, Color.Red, Color.Blue, "reasonable" };
        yield return new object[] { new ListViewItem() { BackColor = Color.Yellow, ForeColor = Color.Yellow, Font = SystemFonts.StatusFont }, string.Empty, Color.Red, Color.Blue, SystemFonts.MenuFont, Color.Red, Color.Blue, string.Empty };

        ListViewItem item = new();
        Assert.Null(item.ListView);
        yield return new object[] { item, "reasonable", Color.Red, Color.Blue, SystemFonts.MenuFont, Color.Red, Color.Blue, "reasonable" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_ListViewItem_String_Color_Color_Font_TestData))]
    public void ListViewSubItem_Ctor_ListViewItem_String_Color_Color_Font(ListViewItem owner, string text, Color foreColor, Color backColor, Font font, Color expectedForeColor, Color expectedBackColor, string expectedText)
    {
        var subItem = new ListViewItem.ListViewSubItem(owner, text, foreColor, backColor, font);
        Assert.Equal(expectedBackColor, subItem.BackColor);
        Assert.Equal(Rectangle.Empty, subItem.Bounds);
        Assert.Equal(font ?? Control.DefaultFont, subItem.Font);
        Assert.Equal(expectedForeColor, subItem.ForeColor);
        Assert.Empty(subItem.Name);
        Assert.Null(subItem.Tag);
        Assert.Equal(expectedText, subItem.Text);
    }

    [WinFormsFact]
    public void ListViewSubItem_BackColor_GetWithListView_ReturnsEqual()
    {
        using ListView listView = new()
        {
            BackColor = Color.Red
        };
        ListViewItem item = new();
        listView.Items.Add(item);

        var subItem = new ListViewItem.ListViewSubItem(item, "text");
        Assert.Equal(Color.Red, subItem.BackColor);

        // Remove from the list view.
        listView.Items.Remove(item);
        Assert.Equal(SystemColors.Window, subItem.BackColor);
    }

    [Fact]
    public void ListViewSubItem_BackColor_GetWithListViewItem_ReturnsEqual()
    {
        ListViewItem item = new()
        {
            BackColor = Color.Red
        };
        var subItem = new ListViewItem.ListViewSubItem(item, "text");
        Assert.Equal(SystemColors.Window, subItem.BackColor);

        // Remove from the list view item.
        item.SubItems.Remove(subItem);
        Assert.Equal(SystemColors.Window, subItem.BackColor);
    }

    [WinFormsFact]
    public void ListViewSubItem_BackColor_GetWithListViewItemWithListView_ReturnsEqual()
    {
        using ListView listView = new()
        {
            BackColor = Color.Red
        };
        ListViewItem item = new()
        {
            BackColor = Color.Blue
        };
        listView.Items.Add(item);
        var subItem = new ListViewItem.ListViewSubItem();
        item.SubItems.Add(subItem);
        Assert.Equal(Color.Red, subItem.BackColor);

        // Remove from the list view item.
        item.SubItems.Remove(subItem);
        Assert.Equal(SystemColors.Window, subItem.BackColor);
    }

    public static IEnumerable<object[]> BackColor_Set_TestData()
    {
        yield return new object[] { Color.Red, Color.Red };
        yield return new object[] { Color.Empty, SystemColors.Window };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void ListViewSubItem_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        var subItem = new ListViewItem.ListViewSubItem
        {
            BackColor = value
        };
        Assert.Equal(expected, subItem.BackColor);

        // Set same.
        subItem.BackColor = value;
        Assert.Equal(expected, subItem.BackColor);
    }

    public static IEnumerable<object[]> BackColor_SetWithListView_TestData()
    {
        yield return new object[] { Color.Green, Color.Green };
        yield return new object[] { Color.Empty, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetWithListView_TestData))]
    public void ListViewSubItem_BackColor_SetWithListViewItemWithListView_GetReturnsExpected(Color value, Color expected)
    {
        using ListView listView = new()
        {
            BackColor = Color.Red
        };
        ListViewItem item = new()
        {
            BackColor = Color.Blue
        };
        listView.Items.Add(item);
        var subItem = new ListViewItem.ListViewSubItem();
        item.SubItems.Add(subItem);

        subItem.BackColor = value;
        Assert.Equal(expected, subItem.BackColor);

        // Set same.
        subItem.BackColor = value;
        Assert.Equal(expected, subItem.BackColor);
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void ListViewSubItem_BackColor_SetWithListViewItem_GetReturnsExpected(Color value, Color expected)
    {
        ListViewItem item = new()
        {
            BackColor = Color.Blue
        };
        var subItem = new ListViewItem.ListViewSubItem();
        item.SubItems.Add(subItem);

        subItem.BackColor = value;
        Assert.Equal(expected, subItem.BackColor);

        // Set same.
        subItem.BackColor = value;
        Assert.Equal(expected, subItem.BackColor);
    }

    [WinFormsFact]
    public void ListViewSubItem_Bounds_GetWithListViewHandle_ReturnsEqual()
    {
        using ListView listView = new();
        ListViewItem item = new();
        listView.Items.Add(item);
        Assert.NotEqual(IntPtr.Zero, listView.Handle);

        var subItem = new ListViewItem.ListViewSubItem(item, "text");
        Assert.Equal(subItem.Bounds, subItem.Bounds);
    }

    [WinFormsFact]
    public void ListViewSubItem_Font_GetWithListView_ReturnsEqual()
    {
        using ListView listView = new()
        {
            Font = SystemFonts.MenuFont
        };
        ListViewItem item = new();
        listView.Items.Add(item);

        var subItem = new ListViewItem.ListViewSubItem(item, "text");
        Assert.Equal(SystemFonts.MenuFont, subItem.Font);

        // Remove from the list view.
        listView.Items.Remove(item);
        Assert.Equal(SystemColors.Window, subItem.BackColor);
    }

    [Fact]
    public void ListViewSubItem_Font_GetWithListViewItem_ReturnsEqual()
    {
        ListViewItem item = new()
        {
            Font = SystemFonts.DialogFont
        };
        var subItem = new ListViewItem.ListViewSubItem(item, "text");
        Assert.Equal(Control.DefaultFont, subItem.Font);

        // Remove from the list view item.
        item.SubItems.Remove(subItem);
        Assert.Equal(Control.DefaultFont, subItem.Font);
    }

    [WinFormsFact]
    public void ListViewSubItem_Font_GetWithListViewItemWithListView_ReturnsEqual()
    {
        using ListView listView = new()
        {
            Font = SystemFonts.CaptionFont
        };
        ListViewItem item = new()
        {
            Font = SystemFonts.DialogFont
        };
        listView.Items.Add(item);

        var subItem = new ListViewItem.ListViewSubItem();
        item.SubItems.Add(subItem);
        Assert.Equal(SystemFonts.CaptionFont, subItem.Font);

        // Remove from the list view item.
        item.SubItems.Remove(subItem);
        Assert.Equal(Control.DefaultFont, subItem.Font);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void ListViewSubItem_Font_Set_GetReturnsExpected(Font value)
    {
        var subItem = new ListViewItem.ListViewSubItem
        {
            Font = value
        };
        Assert.Same(value ?? Control.DefaultFont, subItem.Font);

        // Set same.
        subItem.Font = value;
        Assert.Same(value ?? Control.DefaultFont, subItem.Font);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void ListViewSubItem_Font_SetWithListViewItemWithListView_GetReturnsExpected(Font value)
    {
        using ListView listView = new()
        {
            Font = SystemFonts.CaptionFont
        };
        ListViewItem item = new()
        {
            Font = SystemFonts.DialogFont
        };
        listView.Items.Add(item);
        var subItem = new ListViewItem.ListViewSubItem();
        item.SubItems.Add(subItem);

        subItem.Font = value;
        Assert.Equal(value ?? SystemFonts.CaptionFont, subItem.Font);

        // Set same.
        subItem.Font = value;
        Assert.Equal(value ?? SystemFonts.CaptionFont, subItem.Font);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void ListViewSubItem_Font_SetWithListViewItem_GetReturnsExpected(Font value)
    {
        ListViewItem item = new()
        {
            Font = SystemFonts.DialogFont
        };
        var subItem = new ListViewItem.ListViewSubItem();
        item.SubItems.Add(subItem);

        subItem.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, subItem.Font);

        // Set same.
        subItem.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, subItem.Font);
    }

    [WinFormsFact]
    public void ListViewSubItem_ForeColor_GetWithListView_ReturnsEqual()
    {
        using ListView listView = new()
        {
            ForeColor = Color.Red
        };
        ListViewItem item = new();
        listView.Items.Add(item);

        var subItem = new ListViewItem.ListViewSubItem(item, "text");
        Assert.Equal(Color.Red, subItem.ForeColor);

        // Remove from the list view.
        listView.Items.Remove(item);
        Assert.Equal(SystemColors.WindowText, subItem.ForeColor);
    }

    [Fact]
    public void ListViewSubItem_ForeColor_GetWithListViewItem_ReturnsEqual()
    {
        ListViewItem item = new()
        {
            ForeColor = Color.Red
        };
        var subItem = new ListViewItem.ListViewSubItem(item, "text");
        Assert.Equal(SystemColors.WindowText, subItem.ForeColor);

        // Remove from the list view item.
        item.SubItems.Remove(subItem);
        Assert.Equal(SystemColors.WindowText, subItem.ForeColor);
    }

    [WinFormsFact]
    public void ListViewSubItem_ForeColor_GetWithListViewItemWithListView_ReturnsEqual()
    {
        using ListView listView = new()
        {
            ForeColor = Color.Red
        };
        ListViewItem item = new()
        {
            ForeColor = Color.Blue
        };
        listView.Items.Add(item);
        var subItem = new ListViewItem.ListViewSubItem();
        item.SubItems.Add(subItem);
        Assert.Equal(Color.Red, subItem.ForeColor);

        // Remove from the list view item.
        item.SubItems.Remove(subItem);
        Assert.Equal(SystemColors.WindowText, subItem.ForeColor);
    }

    public static IEnumerable<object[]> ForeColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.WindowText };
        yield return new object[] { Color.White, Color.White };
        yield return new object[] { Color.Black, Color.Black };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_Set_TestData))]
    public void ListViewSubItem_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        var subItem = new ListViewItem.ListViewSubItem
        {
            ForeColor = value
        };
        Assert.Equal(expected, subItem.ForeColor);

        // Set same.
        subItem.ForeColor = value;
        Assert.Equal(expected, subItem.ForeColor);
    }

    public static IEnumerable<object[]> ForeColor_SetWithListView_TestData()
    {
        yield return new object[] { Color.Green, Color.Green };
        yield return new object[] { Color.Empty, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_SetWithListView_TestData))]
    public void ListViewSubItem_ForeColor_SetWithListViewItemWithListView_GetReturnsExpected(Color value, Color expected)
    {
        using ListView listView = new()
        {
            ForeColor = Color.Red
        };
        ListViewItem item = new()
        {
            ForeColor = Color.Blue
        };
        listView.Items.Add(item);
        var subItem = new ListViewItem.ListViewSubItem();
        item.SubItems.Add(subItem);

        subItem.ForeColor = value;
        Assert.Equal(expected, subItem.ForeColor);

        // Set same.
        subItem.ForeColor = value;
        Assert.Equal(expected, subItem.ForeColor);
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_Set_TestData))]
    public void ListViewSubItem_ForeColor_SetWithListViewItem_GetReturnsExpected(Color value, Color expected)
    {
        ListViewItem item = new()
        {
            ForeColor = Color.Blue
        };
        var subItem = new ListViewItem.ListViewSubItem();
        item.SubItems.Add(subItem);

        subItem.ForeColor = value;
        Assert.Equal(expected, subItem.ForeColor);

        // Set same.
        subItem.ForeColor = value;
        Assert.Equal(expected, subItem.ForeColor);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ListViewSubItem_Name_SetWithOwner_GetReturnsExpected(string value, string expected)
    {
        ListViewItem item = new();
        var subItem = new ListViewItem.ListViewSubItem(item, "text")
        {
            Name = value
        };
        Assert.Equal(expected, subItem.Name);

        // Set same.
        subItem.Name = value;
        Assert.Equal(expected, subItem.Name);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ListViewSubItem_Name_SetWithoutOwner_GetReturnsExpected(string value, string expected)
    {
        var subItem = new ListViewItem.ListViewSubItem
        {
            Name = value
        };
        Assert.Equal(expected, subItem.Name);

        // Set same.
        subItem.Name = value;
        Assert.Equal(expected, subItem.Name);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ListViewSubItem_Tag_Set_GetReturnsExpected(string value)
    {
        var subItem = new ListViewItem.ListViewSubItem
        {
            Tag = value
        };
        Assert.Same(value, subItem.Tag);

        // Set same.
        subItem.Tag = value;
        Assert.Same(value, subItem.Tag);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ListViewSubItem_Text_SetWithOwner_GetReturnsExpected(string value, string expected)
    {
        ListViewItem item = new();
        var subItem = new ListViewItem.ListViewSubItem(item, "text")
        {
            Text = value
        };
        Assert.Equal(expected, subItem.Text);

        // Set same.
        subItem.Text = value;
        Assert.Equal(expected, subItem.Text);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ListViewSubItem_Text_SetWithoutOwner_GetReturnsExpected(string value, string expected)
    {
        var subItem = new ListViewItem.ListViewSubItem
        {
            Text = value
        };
        Assert.Equal(expected, subItem.Text);

        // Set same.
        subItem.Text = value;
        Assert.Equal(expected, subItem.Text);
    }

    public static IEnumerable<object[]> ResetStyle_Owner_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ListViewItem() };
    }

    [WinFormsTheory]
    [MemberData(nameof(ResetStyle_Owner_TestData))]
    public void ListViewSubItem_ResetStyle_NoStyle_Nop(ListViewItem owner)
    {
        var subItem = new ListViewItem.ListViewSubItem(owner, "text");
        subItem.ResetStyle();
        Assert.Equal(SystemColors.Window, subItem.BackColor);
        Assert.Equal(Control.DefaultFont, subItem.Font);
        Assert.Equal(SystemColors.WindowText, subItem.ForeColor);

        subItem.ResetStyle();
        Assert.Equal(SystemColors.Window, subItem.BackColor);
        Assert.Equal(Control.DefaultFont, subItem.Font);
        Assert.Equal(SystemColors.WindowText, subItem.ForeColor);
    }

    [WinFormsTheory]
    [MemberData(nameof(ResetStyle_Owner_TestData))]
    public void ListViewSubItem_ResetStyle_HasStyleWithOwner_Success(ListViewItem owner)
    {
        var subItem = new ListViewItem.ListViewSubItem(owner, "text", Color.Red, Color.Blue, SystemFonts.MenuFont);

        subItem.ResetStyle();
        Assert.Equal(SystemColors.Window, subItem.BackColor);
        Assert.Equal(Control.DefaultFont, subItem.Font);
        Assert.Equal(SystemColors.WindowText, subItem.ForeColor);

        subItem.ResetStyle();
        Assert.Equal(SystemColors.Window, subItem.BackColor);
        Assert.Equal(Control.DefaultFont, subItem.Font);
        Assert.Equal(SystemColors.WindowText, subItem.ForeColor);
    }

    public static IEnumerable<object[]> Serialize_Deserialize_TestData()
    {
        yield return new object[] { new ListViewItem.ListViewSubItem() };
        yield return new object[] { new ListViewItem.ListViewSubItem(null, "header", Color.Red, Color.Blue, SystemFonts.MenuFont) { Name = "name", Tag = "tag" } };
    }

    [WinFormsTheory]
    [MemberData(nameof(Serialize_Deserialize_TestData))]
    public void ListViewSubItem_Serialize_Deserialize_Success(ListViewItem.ListViewSubItem subItem)
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.
        new BinaryFormatter().Serialize(stream, subItem);
        stream.Seek(0, SeekOrigin.Begin);

        // cs/dangerous-binary-deserialization
        ListViewItem.ListViewSubItem result = Assert.IsType<ListViewItem.ListViewSubItem>(formatter.Deserialize(stream)); // CodeQL[SM03722] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.
#pragma warning restore SYSLIB0011
        Assert.Equal(subItem.BackColor, result.BackColor);
        Assert.Equal(subItem.Font, result.Font);
        Assert.Equal(subItem.ForeColor, result.ForeColor);
        Assert.Empty(result.Name);
        Assert.Null(result.Tag);
        Assert.Equal(subItem.Text, result.Text);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ListViewSubItem_ToString_Invoke_ReturnsExpected(string text)
    {
        var subItem = new ListViewItem.ListViewSubItem(null, text);
        Assert.Equal($"ListViewSubItem: {{{text}}}", subItem.ToString());
    }
}
