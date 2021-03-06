﻿using System;

namespace org.pescuma.progressmonitor.console.widget
{
    public class TextWidget : ConsoleWidget
    {
        private readonly string text;

        public TextWidget(string text)
        {
            this.text = text;
        }

        public static implicit operator TextWidget(string text)
        {
            return new TextWidget(text);
        }

        public override void Started()
        {
        }

        public override AcceptableSizes ComputeSize(int current, int total, double percent, string[] stepName)
        {
            return new AcceptableSizes(text.Length, text.Length, false);
        }

        public override void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
        {
            writer(text);
        }
    }
}
