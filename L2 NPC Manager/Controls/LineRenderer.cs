using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows;
using System.Windows.Media;

namespace L2NPCManager.Controls
{
    public class LineRenderer : IBackgroundRenderer
    {
        private TextEditor editor;
        private SolidColorBrush brush;


        public LineRenderer(TextEditor editor) {
            this.editor = editor;
            //
            brush = new SolidColorBrush(Color.FromArgb(255, 230, 230, 250));
        }

        //=============================

        public KnownLayer Layer {
            get {return KnownLayer.Background;}
        }

        public void Draw(TextView textView, DrawingContext drawingContext) {
            if (editor.Document == null || textView.ActualWidth <= 0) return;
            //
            Size size;
            textView.EnsureVisualLines();
            var currentLine = editor.Document.GetLineByOffset(editor.CaretOffset);
            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine)) {
                size = new Size(textView.ActualWidth, rect.Height);
                drawingContext.DrawRectangle(brush, null, new Rect(rect.Location, size));
            }
        }
    }
}