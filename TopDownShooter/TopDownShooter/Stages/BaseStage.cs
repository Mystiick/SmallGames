using System;
using System.IO;

using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Markup;

namespace MystiickCore.Models;

public abstract class BaseScreen
{
    public Screen Screen
    {
        get;
        private set;
    }

    public BaseScreen(string markupPath)
    {
        LoadFromMarkup(markupPath);
    }

    private void LoadFromMarkup(string markupPath)
    {
        var parser = new MarkupParser();
        this.Screen = new Screen()
        {
            Content = parser.Parse(
                Path.Combine(AppContext.BaseDirectory, markupPath),
                new object()
            )
        };

        SetupEvents();
    }

    protected virtual void SetupEvents()
    {

    }
}
