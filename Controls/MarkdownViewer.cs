using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace GeminiChat.Controls
{
    /// <summary>
    /// Avalonia control that renders a Markdown string as styled native controls.
    /// Bind the <see cref="Markdown"/> property; the panel rebuilds whenever it changes.
    /// </summary>
    public class MarkdownViewer : StackPanel
    {
        // ── Dependency Property ───────────────────────────────────────────────

        public static readonly StyledProperty<string?> MarkdownProperty =
            AvaloniaProperty.Register<MarkdownViewer, string?>(nameof(Markdown));

        public string? Markdown
        {
            get => GetValue(MarkdownProperty);
            set => SetValue(MarkdownProperty, value);
        }

        // ── Pipeline ─────────────────────────────────────────────────────────

        private static readonly MarkdownPipeline Pipeline =
            new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

        // ── Theme colours (match app palette) ────────────────────────────────

        private static readonly IBrush TextBrush       = new SolidColorBrush(Color.Parse("#E2E8F0"));
        private static readonly IBrush DimBrush        = new SolidColorBrush(Color.Parse("#94A3B8"));
        private static readonly IBrush CodeBg          = new SolidColorBrush(Color.Parse("#0F172A"));
        private static readonly IBrush CodeBorder      = new SolidColorBrush(Color.Parse("#334155"));
        private static readonly IBrush CodeFg          = new SolidColorBrush(Color.Parse("#7DD3FC"));
        private static readonly IBrush InlineCodeBg    = new SolidColorBrush(Color.Parse("#1E293B"));
        private static readonly IBrush QuoteBg         = new SolidColorBrush(Color.Parse("#0F172A"));
        private static readonly IBrush QuoteBar        = new SolidColorBrush(Color.Parse("#3B82F6"));
        private static readonly IBrush HeadingBrush    = new SolidColorBrush(Color.Parse("#F1F5F9"));
        private static readonly IBrush BulletBrush     = new SolidColorBrush(Color.Parse("#60A5FA"));
        private static readonly IBrush RuleBrush       = new SolidColorBrush(Color.Parse("#1E293B"));

        private static readonly FontFamily MonoFont =
            new FontFamily("Cascadia Code,Consolas,Courier New,monospace");

        // ── Constructor ───────────────────────────────────────────────────────

        public MarkdownViewer()
        {
            Spacing   = 6;
            Orientation = Orientation.Vertical;

            // React to property changes
            this.GetObservable(MarkdownProperty).Subscribe(Rebuild);
        }

        // ── Rebuild ───────────────────────────────────────────────────────────

        private void Rebuild(string? md)
        {
            Children.Clear();
            if (string.IsNullOrWhiteSpace(md))
            {
                Children.Add(MakePlainText(md ?? string.Empty));
                return;
            }

            var doc = Markdig.Markdown.Parse(md, Pipeline);
            foreach (var block in doc)
                Children.Add(RenderBlock(block));
        }

        // ── Block rendering ───────────────────────────────────────────────────

        private Control RenderBlock(Block block)
        {
            return block switch
            {
                HeadingBlock h       => RenderHeading(h),
                ParagraphBlock p     => RenderParagraph(p),
                FencedCodeBlock fc   => RenderCodeBlock(fc.Lines.ToString()),
                CodeBlock cb         => RenderCodeBlock(cb.Lines.ToString()),
                QuoteBlock q         => RenderQuote(q),
                ListBlock l          => RenderList(l),
                ThematicBreakBlock _ => RenderRule(),
                _                    => RenderFallback(block)
            };
        }

        // Heading
        private Control RenderHeading(HeadingBlock h)
        {
            var tb = new SelectableTextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Foreground   = HeadingBrush,
                FontWeight   = FontWeight.Bold,
                Margin       = new Thickness(0, 4, 0, 2),
            };
            (tb.FontSize, tb.Margin) = h.Level switch
            {
                1 => (22.0, new Thickness(0, 8, 0, 4)),
                2 => (18.0, new Thickness(0, 6, 0, 3)),
                3 => (15.0, new Thickness(0, 4, 0, 2)),
                _ => (13.5, new Thickness(0, 2, 0, 1)),
            };
            AppendInlines(tb.Inlines!, h.Inline);
            return tb;
        }

        // Paragraph
        private Control RenderParagraph(ParagraphBlock p)
        {
            var tb = new SelectableTextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Foreground   = TextBrush,
                FontSize     = 13.5,
                LineHeight   = 22,
            };
            AppendInlines(tb.Inlines!, p.Inline);
            return tb;
        }

        // Fenced/indented code block
        private Control RenderCodeBlock(string code)
        {
            var scroll = new ScrollViewer
            {
                HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility   = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
            };

            var tb = new SelectableTextBlock
            {
                Text         = code.TrimEnd('\n', '\r'),
                FontFamily   = MonoFont,
                FontSize     = 12,
                Foreground   = CodeFg,
                TextWrapping = TextWrapping.NoWrap,
                Padding      = new Thickness(14, 10),
            };

            scroll.Content = tb;

            return new Border
            {
                Background      = CodeBg,
                BorderBrush     = CodeBorder,
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(8),
                Margin          = new Thickness(0, 2, 0, 2),
                Child           = scroll,
            };
        }

        // Blockquote
        private Control RenderQuote(QuoteBlock q)
        {
            var inner = new StackPanel { Spacing = 4, Margin = new Thickness(10, 6, 6, 6) };
            foreach (var b in q)
                inner.Children.Add(RenderBlock(b));

            return new Grid
            {
                Margin          = new Thickness(0, 2, 0, 2),
                ColumnDefinitions = new ColumnDefinitions("4,*"),
                Children =
                {
                    new Border
                    {
                        Background   = QuoteBar,
                        CornerRadius = new CornerRadius(2),
                        [Grid.ColumnProperty] = 0,
                    },
                    new Border
                    {
                        Background      = QuoteBg,
                        CornerRadius    = new CornerRadius(0, 6, 6, 0),
                        Child           = inner,
                        [Grid.ColumnProperty] = 1,
                    }
                }
            };
        }

        // List
        private Control RenderList(ListBlock list)
        {
            var panel = new StackPanel { Spacing = 3, Margin = new Thickness(4, 0, 0, 0) };
            int index = 1;
            foreach (var item in list)
            {
                if (item is not ListItemBlock li) continue;

                string bullet = list.IsOrdered ? $"{index++}." : "•";

                var bulletText = new TextBlock
                {
                    Text       = bullet,
                    Foreground = BulletBrush,
                    FontSize   = 13.5,
                    Width      = 20,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin     = new Thickness(0, 1, 0, 0),
                };

                var contentPanel = new StackPanel { Spacing = 3 };
                foreach (var b in li)
                    contentPanel.Children.Add(RenderBlock(b));

                var row = new Grid { ColumnDefinitions = new ColumnDefinitions("20,*") };
                row.Children.Add(bulletText);
                Grid.SetColumn(bulletText, 0);
                row.Children.Add(contentPanel);
                Grid.SetColumn(contentPanel, 1);

                panel.Children.Add(row);
            }
            return panel;
        }

        // Horizontal rule
        private Control RenderRule() =>
            new Border
            {
                Height          = 1,
                Background      = RuleBrush,
                Margin          = new Thickness(0, 6, 0, 6),
            };

        // Fallback — just render as plain text
        private Control RenderFallback(Block block)
        {
            var tb = new SelectableTextBlock
            {
                Text         = block.ToString() ?? string.Empty,
                Foreground   = TextBrush,
                FontSize     = 13.5,
                TextWrapping = TextWrapping.Wrap,
            };
            return tb;
        }

        // ── Inline rendering ──────────────────────────────────────────────────

        private void AppendInlines(InlineCollection target, ContainerInline? container)
        {
            if (container == null) return;
            foreach (var inline in container)
                AppendInline(target, inline);
        }

        private void AppendInline(InlineCollection target, Markdig.Syntax.Inlines.Inline inline)
        {
            switch (inline)
            {
                case LiteralInline lit:
                    target.Add(new Run { Text = lit.Content.ToString() });
                    break;

                case EmphasisInline em:
                {
                    // DelimiterCount==2 → bold, ==1 → italic
                    var span = new Span();
                    if (em.DelimiterCount >= 2) span.FontWeight = FontWeight.Bold;
                    if (em.DelimiterCount == 1) span.FontStyle  = FontStyle.Italic;
                    AppendInlines(span.Inlines, em);
                    target.Add(span);
                    break;
                }

                case CodeInline code:
                {
                    // Inline code — render as a bold mono run inside a styled span
                    var span = new Span
                    {
                        FontFamily  = MonoFont,
                        FontSize    = 12,
                        Foreground  = CodeFg,
                        Background  = InlineCodeBg,
                    };
                    span.Inlines.Add(new Run { Text = code.Content });
                    target.Add(span);
                    break;
                }

                case LineBreakInline:
                    target.Add(new LineBreak());
                    break;

                case LinkInline link:
                {
                    // Render link text underlined in blue (no click handler — desktop only)
                    var span = new Span
                    {
                        Foreground      = new SolidColorBrush(Color.Parse("#60A5FA")),
                        TextDecorations = TextDecorations.Underline,
                    };
                    AppendInlines(span.Inlines, link);
                    target.Add(span);
                    break;
                }

                case ContainerInline container:
                    AppendInlines(target, container);
                    break;

                default:
                    target.Add(new Run { Text = inline.ToString() ?? string.Empty });
                    break;
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static SelectableTextBlock MakePlainText(string text) =>
            new()
            {
                Text         = text,
                Foreground   = new SolidColorBrush(Color.Parse("#E2E8F0")),
                FontSize     = 13.5,
                TextWrapping = TextWrapping.Wrap,
                LineHeight   = 22,
            };
    }
}