﻿using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Windows.Controls.Ribbon;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls.Primitives;

namespace Paint.ViewModel
{

    enum DrawType {nothing, pencil, brush, line, ellipse, rectangle, triangle, arrow, heart, fill, erase, text, bucket };
    public class MainWindowViewModel : Control, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private Line curLine;
        private Shape curShape;
        private int Outline = 1;
        private int  NumberUndo = 1, NumberRedo = 1;
        private string CurrentPath = null;
        private int Isdelete = 0;
        private ContentControl curControl;
        private TextBox txtCurrentTextbox, txtFocus = new TextBox();
        private System.Windows.Media.FontFamily fontFamily;
        private double fontSize;
        private System.Windows.FontStyle fontStyle;
        private FontWeight fontWeight;
        private TextDecorationCollection decoration;
        private System.Windows.Media.Color fontColor;
        bool bold = false, italic = false, underlined = false;
        private bool IsShape = false;
        private bool IsColor1 = true;
        private bool IsCheckFill = false;
        private bool isCanvas_MouseDown = false;
        System.Windows.Point CurrentPointDown = new System.Windows.Point();
        System.Windows.Point CurrentPointMove = new System.Windows.Point();
        private ICommand _Canvas_MouseDown;
        private System.Windows.Media.Brush _color1 = new SolidColorBrush(Colors.Black), _color2 = new SolidColorBrush(Colors.White);
        private System.Windows.Media.Brush _colorFill;
        private int StrokeThickness = 1;
        private int one = 1, two = 2, three = 3;
        private bool isItemMenu = false;
      
        private ICommand _Canvas_MouseMove;
        private ICommand _Canvas_MouseUp;
        private ICommand _StrokeThicknessCommand;
        private ICommand _StrokeCommand;
        private ICommand _ChooseColor1Command;
        private ICommand _ChooseColor2Command;
        private ICommand _ColorCommand;
        private ICommand _LineCommand;
        private ICommand _BrushCommand;
        private ICommand _EraserCommand;
        private ICommand _PenCommand;
        private ICommand _SmoothCommand;
        private ICommand _DarkCommand;
        private ICommand _MixCommand;
        private ICommand _RectangleCommand;
        private ICommand _TriangleCommand;
        private ICommand _ArrowCommand;
        private ICommand _CircleCommand;
        private ICommand _HeartCommand;
        private ICommand _FillCommand;
        private ICommand _BucketCommand;
        private ICommand _UndoCommand;
        private ICommand _RedoCommand;
        private ICommand _OpenFileCommand;
        private ICommand _SaveFileCommand;
        private ICommand _NewFileCommand;
        private ICommand _ExitCommand;
        private ICommand _UndoNumberCommand;
        private ICommand _RedoNumberCommand;
        private ICommand _TextCommand;
        private ICommand _BoldCommand;
        private ICommand _ItalicCommand;
        private ICommand _UnderlinedCommand;
        private ICommand _FontFamilycommand;
        private ICommand _SizeTextCommand;
        private ICommand _NothingCommand;
        private ICommand _PreviewMouseLeftButtonDownSelectCommand;
        private ICommand _PreviewMouseUpButtonDownSelectCommand;
        public MainWindowViewModel()
        {
            ColorFill = Color1;
            drawType = DrawType.brush;
            
            fontSize = 14;
            fontFamily = new System.Windows.Media.FontFamily("Arial");
            fontColor = (System.Windows.Media.Color)(Color1.GetValue(SolidColorBrush.ColorProperty));
            fontStyle = FontStyles.Normal;
            fontWeight = FontWeights.Normal;
            decoration = null;
        }
        public ICommand Canvas_MouseUp
        {
            get
            {
                _Canvas_MouseUp = new RelayCommand<Canvas>((p) => true, OnCanvas_MouseUp);
                return _Canvas_MouseUp;
            }
            set
            {
                _Canvas_MouseUp = value;
            }
        }

        private void OnCanvas_MouseUp(Canvas canvas)
        {
            if (CurrentPage.Instance.STT < CurrentPage.Instance.StkShape.Count - 1)
            {
                CurrentPage.Instance.StkShape.RemoveRange(CurrentPage.Instance.STT, CurrentPage.Instance.StkShape.Count - (CurrentPage.Instance.STT));
            }
            isCanvas_MouseDown = false;
            if (drawType == DrawType.line)
            {
                curControl = new ContentControl();
                Line Line = new Line();

                Line.X1 = CurrentPointDown.X;
                Line.Y1 = CurrentPointDown.Y;
                Line.X2 = CurrentPointMove.X;
                Line.Y2 = CurrentPointMove.Y;
                try
                {
                    Line.StrokeThickness = StrokeThickness;
                    Line.Stroke = Color1;
                    Canvas.SetLeft(curControl, Line.Margin.Left);
                    Canvas.SetTop(curControl, Line.Margin.Top);
                    curControl.Width = Line.Width;
                    curControl.Height = Line.Height;
                    curControl.Content = Line;
                    curControl.Background = Color1;
                    curControl.Style = canvas.Resources["DesignerItemStyle"] as Style;
                    CurrentPage.Instance.DrawShape(curControl, Outline, canvas);

                    curLine = null;
                }
                catch(Exception e) { MessageBox.Show(e.Message); }
            }
            else if (drawType == DrawType.ellipse || drawType == DrawType.rectangle || drawType == DrawType.triangle || drawType == DrawType.arrow || drawType == DrawType.heart)
            {
                curControl = new ContentControl();
                Shape temp;
                if (drawType == DrawType.ellipse)
                {
                    temp = new Ellipse();

                }
                else if (drawType == DrawType.rectangle)
                {
                    temp = new System.Windows.Shapes.Rectangle();
                }
                else if (drawType == DrawType.triangle)
                {
                    temp = new Triangle();
                    ((Triangle)temp).Start = ((Triangle)curShape).Start;
                    temp.Width = curShape.Width;
                    temp.Height = curShape.Height;
                }
                else if (drawType == DrawType.arrow)
                {
                    temp = new Arrow();
                    ((Arrow)temp).Start = ((Arrow)curShape).Start;
                    temp.Width = curShape.Width;
                    temp.Height = curShape.Height;
                }
                else
                {
                    temp = new Heart();
                    ((Heart)temp).Start = ((Heart)curShape).Start;
                    temp.Width = curShape.Width;
                    temp.Height = curShape.Height;
                }
                try
                {
                    temp.Stroke = Color1;
                    temp.StrokeThickness = StrokeThickness;
                    temp.IsHitTestVisible = true;
                    if (IsCheckFill) temp.Fill = ColorFill;
                    Canvas.SetLeft(curControl, curShape.Margin.Left);
                    Canvas.SetTop(curControl, curShape.Margin.Top);
                    curControl.Width = curShape.Width;
                    curControl.Height = curShape.Height;
                    curControl.Content = temp;
                    curControl.Background = Color1;
                    curControl.Style = canvas.Resources["DesignerItemStyle"] as Style;
                    CurrentPage.Instance.DrawShape(curControl, Outline, canvas);


                    curShape = null;
                }
                catch(Exception e) { MessageBox.Show(e.Message); }
            }
            else if (drawType == DrawType.bucket)
            {
                try
                {
                    System.Drawing.Color color = new System.Drawing.Color();
                    color = System.Drawing.Color.FromArgb(((System.Windows.Media.Color)(Color1.GetValue(SolidColorBrush.ColorProperty))).A,
                        ((System.Windows.Media.Color)(Color1.GetValue(SolidColorBrush.ColorProperty))).R,
                        ((System.Windows.Media.Color)(Color1.GetValue(SolidColorBrush.ColorProperty))).G,
                        ((System.Windows.Media.Color)(Color1.GetValue(SolidColorBrush.ColorProperty))).B);
                    Bitmap bm = CurrentPage.Instance.CanvasToBitmap(canvas);
                    CurrentPage.Instance.FloodFill(bm, new System.Drawing.Point((int)Mouse.GetPosition(canvas).X, (int)Mouse.GetPosition(canvas).Y), color, canvas);
                }
                catch (Exception e) { MessageBox.Show(e.Message); }



            }
            else if (drawType == DrawType.text)
            {
                try
                {
                    txtCurrentTextbox.Focus();
                    
                }
                catch { }
            }
            else
            {
                CurrentPage.Instance.RefreshCanvas(canvas);
            }

        }

        public ICommand Canvas_MouseMove
        {
            get
            {
                _Canvas_MouseMove = new RelayCommand<Canvas>((p) => true, OnCanvas_MouseMove);
                return _Canvas_MouseMove;
            }

            set
            {
                _Canvas_MouseMove = value;
            }
        }

        private void OnCanvas_MouseMove(Canvas canvas)
        {
            CurrentPointMove = Mouse.GetPosition(canvas);
            bool addShape = false;
            if (isCanvas_MouseDown == true)
            {

                if ((drawType == DrawType.ellipse || drawType == DrawType.rectangle || drawType == DrawType.triangle || drawType == DrawType.arrow || drawType == DrawType.heart) && IsShape)
                {
                    txtFocus.Focus();
                    if (curShape == null)
                    {

                        if (drawType == DrawType.ellipse)
                        {
                            curShape = new Ellipse();
                        }
                        else if (drawType == DrawType.rectangle)
                        {
                            curShape = new System.Windows.Shapes.Rectangle();
                        }
                        else if (drawType == DrawType.triangle)
                        {
                            curShape = new Triangle();
                            ((Triangle)curShape).Start = CurrentPointDown;
                        }
                        else if (drawType == DrawType.arrow)
                        {

                            curShape = new Arrow();
                            ((Arrow)curShape).Start = CurrentPointDown;
                        }
                        else
                        {
                            curShape = new Heart();
                            ((Heart)curShape).Start = CurrentPointDown;
                        }
                        addShape = true;
                        curShape.StrokeThickness = StrokeThickness;
                        curShape.Stroke = Color1;
                    }

                    if (CurrentPointMove.X <= CurrentPointDown.X && CurrentPointMove.Y <= CurrentPointDown.Y)  //Góc phần tư thứ nhất
                    {
                        curShape.Margin = new Thickness(CurrentPointMove.X, CurrentPointMove.Y, 0, 0);
                    }
                    else if (CurrentPointMove.X >= CurrentPointDown.X && CurrentPointMove.Y <= CurrentPointDown.Y)
                    {
                        curShape.Margin = new Thickness(CurrentPointDown.X, CurrentPointMove.Y, 0, 0);
                    }
                    else if (CurrentPointMove.X >= CurrentPointDown.X && CurrentPointMove.Y >= CurrentPointDown.Y)
                    {
                        curShape.Margin = new Thickness(CurrentPointDown.X, CurrentPointDown.Y, 0, 0);
                    }
                    else if (CurrentPointMove.X <= CurrentPointDown.X && CurrentPointMove.Y >= CurrentPointDown.Y)
                    {
                        curShape.Margin = new Thickness(CurrentPointDown.X, CurrentPointDown.Y, 0, 0);
                    }
                    if (IsCheckFill) curShape.Fill = ColorFill;
                    curShape.Width = Math.Abs(CurrentPointMove.X - CurrentPointDown.X);
                    curShape.Height = Math.Abs(CurrentPointMove.Y - CurrentPointDown.Y);


                    if (addShape)
                        CurrentPage.Instance.DrawCapture(curShape, canvas);
                }
                else
                    if (drawType == DrawType.brush || drawType == DrawType.erase || drawType == DrawType.pencil)
                {
                    txtFocus.Focus();
                    Line line = new Line();
                    line.Stroke = Color1;

                    if (drawType == DrawType.erase)
                    {
                        line.Stroke = Color2;
                        StrokeThickness = 15;
                    }
                    else if (drawType == DrawType.brush && !isItemMenu)
                    {
                        StrokeThickness = 3;
                    }
                    else if (!isItemMenu)
                        StrokeThickness = 1;
                    line.StrokeThickness = StrokeThickness;
                    line.X1 = CurrentPointDown.X;
                    line.Y1 = CurrentPointDown.Y;
                    line.X2 = Mouse.GetPosition(canvas).X;
                    line.Y2 = Mouse.GetPosition(canvas).Y;


                    CurrentPointDown = Mouse.GetPosition(canvas);
                    canvas.Children.Add(line);


                }
                else if (drawType == DrawType.line)
                {
                    txtFocus.Focus();
                    if (curLine == null)
                    {
                        curLine = new Line();
                        addShape = true;
                    }
                    curLine.X1 = CurrentPointDown.X;
                    curLine.Y1 = CurrentPointDown.Y;
                    curLine.X2 = CurrentPointMove.X;
                    curLine.Y2 = CurrentPointMove.Y;

                    curLine.StrokeThickness = StrokeThickness;
                    curLine.Stroke = Color1;

                    if (addShape)
                    {
                        CurrentPage.Instance.DrawCapture(curLine, canvas);
                    }
                }
                else if (drawType == DrawType.text)
                {
                    if (txtCurrentTextbox == null)
                    {
                        txtCurrentTextbox = new TextBox();
                        txtCurrentTextbox.FontFamily = fontFamily;
                        txtCurrentTextbox.FontSize = fontSize;
                        txtCurrentTextbox.FontStyle = fontStyle;
                        txtCurrentTextbox.FontWeight = fontWeight;
                        txtCurrentTextbox.TextWrapping = TextWrapping.Wrap;
                        txtCurrentTextbox.AcceptsReturn = true;
                        if (decoration != null)
                        {
                            txtCurrentTextbox.TextDecorations = decoration;
                        }

                        txtCurrentTextbox.TextWrapping = TextWrapping.Wrap;
                        txtCurrentTextbox.LostFocus += TxtCurrentTextbox_LostFocus;
                        txtCurrentTextbox.SelectionChanged += TxtCurrentTextbox_SelectionChanged;
                        addShape = true;
                    }
                    txtCurrentTextbox.Margin = new Thickness(CurrentPointDown.X, CurrentPointDown.Y, 0, 0);
                    txtCurrentTextbox.MinHeight = Math.Abs(CurrentPointMove.Y - CurrentPointDown.Y);
                    txtCurrentTextbox.Width = Math.Abs(CurrentPointMove.X - CurrentPointDown.X);

                    if (addShape)
                    {
                        canvas.Children.Add(txtCurrentTextbox);
                    }
                }
            }

        }

        private void TxtCurrentTextbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                fontColor = (System.Windows.Media.Color)(Color1.GetValue(SolidColorBrush.ColorProperty));
                txtCurrentTextbox.Foreground = new SolidColorBrush(fontColor);
                txtCurrentTextbox.FontFamily = fontFamily;
                txtCurrentTextbox.FontSize = fontSize;
                txtCurrentTextbox.FontStyle = fontStyle;
                txtCurrentTextbox.FontWeight = fontWeight;
                if (decoration != null)
                {
                    txtCurrentTextbox.TextDecorations = decoration;
                }
            }
            catch { }
            
        }

        private void TxtCurrentTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).BorderThickness = new Thickness(0);
            (sender as TextBox).IsReadOnly = true;
            (sender as TextBox).Background = System.Windows.Media.Brushes.Transparent;
            txtCurrentTextbox = null;
        }

        public ICommand Canvas_MouseDown
        {
            get
            {
                _Canvas_MouseDown = new RelayCommand<Canvas>((p) => true, OnCanvas_MouseDown);
                return _Canvas_MouseDown;
            }
            set
            {
                _Canvas_MouseDown = value;
            }
        }
        private void OnCanvas_MouseDown(Canvas canvas)
        {
            CurrentPointDown = Mouse.GetPosition(canvas);

            isCanvas_MouseDown = true;
            if (drawType == DrawType.text)
            {

                try
                {
                    canvas.Children.Add(txtFocus);
                }
                catch { }
                txtFocus.BorderThickness = new Thickness(0);
                txtFocus.Focus();

            }



        }
        public System.Windows.Media.Brush Color1
        {
            get
            {
                return _color1;
            }

            set
            {
                _color1 = value; NotifyPropertyChanged("Color1");
            }
        }

        public System.Windows.Media.Brush Color2
        {
            get
            {
                return _color2;
            }

            set
            {
                _color2 = value; NotifyPropertyChanged("Color2");
            }
        }

        public ICommand StrokeThicknessCommand
        {
            get
            {
                _StrokeThicknessCommand = new RelayCommand<RibbonMenuItem>((p) => true, OnStrokeThicknessCommand);
                return _StrokeThicknessCommand;
            }

            set
            {
                _StrokeThicknessCommand = value;
            }
        }

        public ICommand StrokeCommand
        {
            get
            {
                _StrokeCommand = new RelayCommand<RibbonButton>((p) => true, OnStrokeCommand);
                return _StrokeCommand;
            }

            set
            {
                _StrokeCommand = value;
            }
        }

        public ICommand ChooseColor1Command
        {
            get
            {
                _ChooseColor1Command = new RelayCommand<object>((p) => true, OnChooseColor1Command);
                return _ChooseColor1Command;
            }

            set
            {
                _ChooseColor1Command = value;
            }
        }

        public ICommand ChooseColor2Command
        {
            get
            {
                _ChooseColor2Command = new RelayCommand<object>((p) => true, OnChooseColor2Command);
                return _ChooseColor2Command;
            }

            set
            {
                _ChooseColor2Command = value;
            }
        }

        public ICommand ColorCommand
        {
            get
            {
                _ColorCommand = new RelayCommand<RibbonButton>((p) => true, OnColorCommand);


                return _ColorCommand;
            }

            set
            {
                _ColorCommand = value;
            }
        }

        public ICommand LineCommand
        {
            get
            {
                _LineCommand = new RelayCommand<Canvas>((p) => true, OnLineCommand);
                return _LineCommand;
            }

            set
            {
                _LineCommand = value;
            }
        }

        public ICommand BrushCommand
        {
            get
            {
                _BrushCommand = new RelayCommand<Canvas>((p) => true, OnBrushCommand);
                return _BrushCommand;
            }

            set
            {
                _BrushCommand = value;
            }
        }

        public ICommand EraserCommand
        {
            get
            {
                _EraserCommand = new RelayCommand<Canvas>((p) => true, OnEraserCommand);
                return _EraserCommand;
            }

            set
            {
                _EraserCommand = value;
            }
        }

        public ICommand PenCommand
        {
            get
            {
                _PenCommand = new RelayCommand<Canvas>((p) => true, OnPenCommand);
                return _PenCommand;
            }

            set
            {
                _PenCommand = value;
            }
        }

        public Canvas Canvas
        {
            get { return (Canvas)GetValue(MainWindowViewModel.CanvasProperty); }
            set { SetValue(MainWindowViewModel.CanvasProperty, value); }
        }

        public ICommand SmoothCommand
        {
            get
            {
                _SmoothCommand = new RelayCommand<object>((p) => true, OnSmoothCommand);
                return _SmoothCommand;
            }

            set
            {
                _SmoothCommand = value;
            }
        }

        private void OnSmoothCommand(object obj)
        {
            Outline = 1;
        }

        public ICommand DarkCommand
        {
            get
            {
                _DarkCommand = new RelayCommand<object>((p) => true, OnDarkCommand);
                return _DarkCommand;
            }

            set
            {
                _DarkCommand = value;
            }
        }

        private void OnDarkCommand(object obj)
        {
            Outline = 2;
        }

        public ICommand MixCommand
        {
            get
            {
                _MixCommand = new RelayCommand<object>((p) => true, OnMixCommand);
                return _MixCommand;
            }

            set
            {
                _MixCommand = value;
            }
        }

        public ICommand RectangleCommand
        {
            get
            {
                _RectangleCommand = new RelayCommand<Canvas>((p) => true, OnRectangleCommand);
                return _RectangleCommand;
            }

            set
            {
                _RectangleCommand = value;
            }
        }

        public ICommand TriangleCommand
        {
            get
            {
                _TriangleCommand = new RelayCommand<Canvas>((p) => true, OnTriangleCommand);
                return _TriangleCommand;
            }

            set
            {
                _TriangleCommand = value;
            }
        }

        public ICommand ArrowCommand
        {
            get
            {
                _ArrowCommand = new RelayCommand<Canvas>((p) => true, OnArrowCommand);
                return _ArrowCommand;
            }

            set
            {
                _ArrowCommand = value;
            }
        }

        public ICommand CircleCommand
        {
            get
            {
                _CircleCommand = new RelayCommand<Canvas>((p) => true, OnCircleCommand);
                return _CircleCommand;
            }

            set
            {
                _CircleCommand = value;
            }
        }

        public ICommand HeartCommand
        {
            get
            {
                _HeartCommand = new RelayCommand<Canvas>((p) => true, OnHeartCommand);
                return _HeartCommand;
            }

            set
            {
                _HeartCommand = value;
            }
        }

        public System.Windows.Media.Brush ColorFill
        {
            get
            {
                return _colorFill;
            }
            set
            {
                _colorFill = value; NotifyPropertyChanged("ColorFill");
            }
        }

        public ICommand FillCommand
        {
            get
            {
                _FillCommand = new RelayCommand<RibbonCheckBox>((p) => true, OnFillCommand);
                return _FillCommand;
            }

            set
            {
                _FillCommand = value;
            }
        }

        public ICommand BucketCommand
        {
            get
            {
                _BucketCommand = new RelayCommand<Canvas>((p) => true, OnBucketCommand);
                return _BucketCommand;
            }

            set
            {
                _BucketCommand = value;
            }
        }

        public ICommand UndoCommand
        {
            get
            {
                _UndoCommand = new RelayCommand<Canvas>((p) => true, OnUndoCommand);
                return _UndoCommand;
            }

            set
            {
                _UndoCommand = value;
            }
        }

        public ICommand RedoCommand
        {
            get
            {
                _RedoCommand = new RelayCommand<Canvas>((p) => true, OnRedoCommand);
                return _RedoCommand;
            }

            set
            {
                _RedoCommand = value;
            }
        }

        public ICommand OpenFileCommand
        {
            get
            {
                _OpenFileCommand = new RelayCommand<Canvas>((p) => true, OnOpenFileCommand);
                return _OpenFileCommand;
            }

            set
            {
                _OpenFileCommand = value;
            }
        }

        public ICommand SaveFileCommand
        {
            get
            {
                _SaveFileCommand = new RelayCommand<Canvas>((p) => true, OnSaveFileCommand);
                return _SaveFileCommand;
            }

            set
            {
                _SaveFileCommand = value;
            }
        }

        public ICommand NewFileCommand
        {
            get
            {
                _NewFileCommand = new RelayCommand<Canvas>((p) => true, OnNewFileCommand);
                return _NewFileCommand;
            }

            set
            {
                _NewFileCommand = value;
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                _ExitCommand = new RelayCommand<Canvas>((p) => true, OnExitCommand);
                return _ExitCommand;
            }

            set
            {
                _ExitCommand = value;
            }
        }

        public ICommand UndoNumberCommand
        {
            get
            {
                _UndoNumberCommand = new RelayCommand<int>((p) => OnUndoNumberCommand(p));
                return _UndoNumberCommand;
            }

            set
            {
                _UndoNumberCommand = value;
            }
        }

        public ICommand RedoNumberCommand
        {
            get
            {
                _RedoNumberCommand = new RelayCommand<int>((p) => OnRedoNumberCommand(p));
                return _RedoNumberCommand;
            }

            set
            {
                _RedoNumberCommand = value;
            }
        }

        public int One
        {
            get
            {
                return one;
            }

            set
            {
                one = value;
            }
        }

        public int Two
        {
            get
            {
                return two;
            }

            set
            {
                two = value;
            }
        }

        public int Three
        {
            get
            {
                return three;
            }

            set
            {
                three = value;
            }
        }

        public ICommand TextCommand
        {
            get
            {
                _TextCommand = new RelayCommand<Canvas>((p) => true, OnTextCommand);
                return _TextCommand;
            }

            set
            {
                _TextCommand = value;
            }
        }

        public ICommand BoldCommand
        {
            get
            {
                _BoldCommand = new RelayCommand<RibbonRadioButton>((p) => true, OnBoldCommand);
                return _BoldCommand;
            }

            set
            {
                _BoldCommand = value;
            }
        }

        public ICommand ItalicCommand
        {
            get
            {
                _ItalicCommand = new RelayCommand<RibbonRadioButton>((p) => true, OnItalicCommand);
                return _ItalicCommand;
            }

            set
            {
                _ItalicCommand = value;
            }
        }

        public ICommand UnderlinedCommand
        {
            get
            {
                _UnderlinedCommand = new RelayCommand<RibbonRadioButton>((p) => true, OnUnderlinedCommand);
                return _UnderlinedCommand;
            }

            set
            {
                _UnderlinedCommand = value;
            }
        }

        public bool Bold
        {
            get
            {
                return bold;
            }

            set
            {
                bold = value;NotifyPropertyChanged("Bold");
            }
        }

        public bool Italic
        {
            get
            {
                return italic;
            }

            set
            {
                italic = value; NotifyPropertyChanged("Italic");
            }
        }

        public bool Underlined
        {
            get
            {
                return underlined;
            }

            set
            {
                underlined = value; NotifyPropertyChanged("Underlined");
            }
        }

        public ICommand FontFamilycommand
        {
            get
            {
                _FontFamilycommand = new RelayCommand<ComboBox>((p) => true, OnFontFamilyCommand);
                return _FontFamilycommand;
            }

            set
            {
                _FontFamilycommand = value;
            }
        }

        public ICommand SizeTextCommand
        {
            get
            {
                _SizeTextCommand = new RelayCommand<ComboBox>((p) => true, OnSizeTextCommand);
                return _SizeTextCommand;
            }

            set
            {
                _SizeTextCommand = value;
            }
        }

        public ICommand PreviewMouseLeftButtonDownSelectCommand
        {
            get
            {
                _PreviewMouseLeftButtonDownSelectCommand = new RelayCommand<Canvas>((p) => true, OnPreviewMouseLeftButtonDownSelectCommand);
                return _PreviewMouseLeftButtonDownSelectCommand;
            }

            set
            {
                _PreviewMouseLeftButtonDownSelectCommand = value;
            }
        }

        public ICommand NothingCommand
        {
            get
            {
                _NothingCommand = new RelayCommand<Canvas>((p) => true, OnNothingCommand);
                return _NothingCommand;
            }

            set
            {
                _NothingCommand = value;
            }
        }

        public ICommand PreviewMouseUpButtonDownSelectCommand
        {
            get
            {
                _PreviewMouseLeftButtonDownSelectCommand = new RelayCommand<Canvas>((p) => true, OnPreviewMouseUpButtonDownSelectCommand);
                return _PreviewMouseUpButtonDownSelectCommand;
            }

            set
            {
                _PreviewMouseUpButtonDownSelectCommand = value;
            }
        }

        private void OnPreviewMouseUpButtonDownSelectCommand(Canvas canvas)
        {
            Selector.SetIsSelected(canvas, false);
        }

        private void OnNothingCommand(Canvas obj)
        {
            obj.Cursor = Cursors.Arrow;
            drawType = DrawType.nothing;
        }

        private void OnPreviewMouseLeftButtonDownSelectCommand(Canvas canvas)
        {
            if (drawType == DrawType.nothing)
            {
                Selector.SetIsSelected(canvas.Children[1], true);
            }
        }

        private void OnSizeTextCommand(ComboBox obj)
        {
            ComboBoxItem ComboItem = (ComboBoxItem)obj.SelectedItem;
            string value = ComboItem.Content.ToString();
            fontSize = Convert.ToDouble(value);
            if (txtCurrentTextbox!=null)
            {
                txtCurrentTextbox.FontSize = fontSize;
            }
        }

        private void OnFontFamilyCommand(ComboBox obj)
        {
            fontFamily = new System.Windows.Media.FontFamily(obj.SelectedItem.ToString());
            if (txtCurrentTextbox != null)
            {
                txtCurrentTextbox.FontFamily = fontFamily;
            }
        }

        int k = 0;
        private void OnUnderlinedCommand(RibbonRadioButton obj)
        {
            if (k == 0) { Underlined = true; k = 1; }
            else { Underlined = false; k = 0; }
            if (Underlined)
            {
                if (txtCurrentTextbox != null)
                {
                    if (txtCurrentTextbox.TextDecorations == null)
                    {
                        txtCurrentTextbox.TextDecorations = new TextDecorationCollection();
                    }
                    txtCurrentTextbox.TextDecorations = TextDecorations.Underline;
                }
            }
            else
            {
                if (txtCurrentTextbox != null)
                {
                    txtCurrentTextbox.TextDecorations = null;
                }
            }
            
        }
        int j = 0;
        private void OnItalicCommand(RibbonRadioButton obj)
        {
            if (j == 0) { Italic = true; j = 1; }
            else { Italic = false; j = 0; }
            if (Italic)
            { fontStyle = FontStyles.Italic; txtCurrentTextbox.FontStyle = FontStyle; }
            else { fontStyle = FontStyles.Normal; }
            txtCurrentTextbox.FontStyle = FontStyle;
        }
        int i = 0;
        private void OnBoldCommand(RibbonRadioButton obj)
        {
            if (i == 0) { Bold = true; i = 1; }
            else { Bold = false; i = 0; }
            if (Bold)
            { fontWeight = FontWeights.Bold; }
            else { fontWeight = FontWeights.Normal; }
            txtCurrentTextbox.FontWeight = FontWeight;
        }

        private void OnTextCommand(Canvas canvas)
        {
            drawType = DrawType.text;
            canvas.Cursor = Cursors.IBeam;
        }

        private void OnRedoNumberCommand(int numberRedo)
        {
            NumberRedo = numberRedo;
        }

        private void OnUndoNumberCommand(int numberUndo)
        {
            NumberUndo = numberUndo;
        }

        private void OnExitCommand(Canvas canvas)
        {
            if (CurrentPath == null && canvas.Children.Count == 0)
                Application.Current.Shutdown();
            else if (canvas.Children.Count > 0)
            {
                if (MessageBox.Show("Bạn có muốn lưu không", "Lưu ý", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    OnSaveFileCommand(canvas);
                    Application.Current.Shutdown();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }

        }

        private void OnNewFileCommand(Canvas canvas)
        {
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Height = canvas.ActualHeight;
            img.Width = canvas.ActualWidth;
            canvas.Children.Clear();
            canvas.Children.Add(img);
        }

        private void OnSaveFileCommand(Canvas canvas)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas);
            double dpi = 96d;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }
            rtb.Render(dv);
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));
            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                pngEncoder.Save(ms);

                ms.Close();
                ms.Dispose();
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.Title = "Save as";
                dlg.Filter = "Bitmap files (*.bmp)|*.bmp|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG (*.png)|*.png|All files (*.*)|*.*";
                if (CurrentPath == null)
                {
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string fileName = dlg.FileName;
                        System.IO.File.WriteAllBytes(fileName, ms.ToArray());
                    }
                }
                else
                {
                    System.IO.File.WriteAllBytes(CurrentPath, ms.ToArray());
                }
                CurrentPath = dlg.FileName;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnOpenFileCommand(Canvas canvas)
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Title = "Choose an image file";
            dlg.Filter = "Bitmap files (*.bmp)|*.bmp|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG (*.png)|*.png|All files (*.*)|*.*";
            try
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ImageBrush brush = new ImageBrush();
                    BitmapImage img = new BitmapImage(new Uri(dlg.FileName, UriKind.Relative));
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(img));
                    string tempPath = CreateTempFile();
                    using (var stream = System.IO.File.Open(tempPath, System.IO.FileMode.Open))
                    {
                        encoder.Save(stream);
                        stream.Close();
                    }
                    BitmapImage temp = new BitmapImage(new Uri(tempPath, UriKind.Relative));
                    brush.ImageSource = temp;
                    canvas.Children.Clear();
                    canvas.Background = brush;

                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: Could not read file from disk.\nOriginal error: " + ex.Message);
            }
            CurrentPath = dlg.FileName;
        }

        private string CreateTempFile()
        {
            string fileName = string.Empty;

            try
            {
                fileName = System.IO.Path.GetTempFileName();

                // Create a FileInfo object to set the file's attributes
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);

                // Set the Attribute property of this file to Temporary. 
                fileInfo.Attributes = System.IO.FileAttributes.Temporary;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to create tempfile\nDetail: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return fileName;
        }

        private void OnRedoCommand(Canvas canvas)
        {

            if (CurrentPage.Instance.STT + NumberRedo - 1 < CurrentPage.Instance.StkShape.Count)
            {
                CurrentPage.Instance.STT += NumberRedo;
                canvas.Children.Clear();
                canvas.Children.Add(CurrentPage.Instance.StkShape[CurrentPage.Instance.STT - 1]);
            }

        }

        private void OnUndoCommand(Canvas canvas)
        {
            if (CurrentPage.Instance.STT == CurrentPage.Instance.StkShape.Count && Isdelete == 0)
            {
                canvas.Children.Remove(curControl);
                Isdelete = 1;
            }
            else if (CurrentPage.Instance.STT - NumberUndo > 0 && Isdelete == 1)
            {
                CurrentPage.Instance.STT -= NumberUndo;
                canvas.Children.Clear();
                canvas.Children.Add(CurrentPage.Instance.StkShape[CurrentPage.Instance.STT - NumberUndo]);
            }
            else if (CurrentPage.Instance.STT - NumberUndo <= 0)
            {
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                img.Width = canvas.ActualWidth;
                img.Height = canvas.ActualHeight;
                canvas.Children.Clear();
                canvas.Children.Add(img);
            }
        }

        private void OnBucketCommand(Canvas canvas)
        {
            drawType = DrawType.bucket;
            canvas.Cursor = Cursors.Arrow;
        }

        private void OnFillCommand(RibbonCheckBox cbk)
        {
            if (cbk.IsChecked == true)
            {
                IsCheckFill = true;
            }
            else IsCheckFill = false;
        }

        private void OnHeartCommand(Canvas obj)
        {
            drawType = DrawType.heart;
            obj.Cursor = Cursors.Cross;
            IsShape = true;
        }

        private void OnCircleCommand(Canvas obj)
        {
            drawType = DrawType.ellipse;
            obj.Cursor = Cursors.Cross;
            IsShape = true;
        }

        private void OnArrowCommand(Canvas obj)
        {
            drawType = DrawType.arrow;
            obj.Cursor = Cursors.Cross;
            IsShape = true;
        }

        private void OnTriangleCommand(Canvas obj)
        {
            drawType = DrawType.triangle;
            obj.Cursor = Cursors.Cross;
            IsShape = true;
        }

        private void OnRectangleCommand(Canvas obj)
        {
            drawType = DrawType.rectangle;
            obj.Cursor = Cursors.Cross;
            IsShape = true;
        }

        private void OnMixCommand(object obj)
        {
            Outline = 3;
        }

        public static readonly DependencyProperty CanvasProperty =
            DependencyProperty.Register("Canvas", typeof(Canvas), typeof(MainWindowViewModel));


        private void OnPenCommand(Canvas obj)
        {
            isItemMenu = false;
            drawType = DrawType.pencil;
            obj.Cursor = Cursors.Pen;
        }

        private void OnEraserCommand(Canvas obj)
        {
            isItemMenu = false;
            drawType = DrawType.erase;
            obj.Cursor = Cursors.Arrow;
        }

        private void OnBrushCommand(Canvas obj)
        {
            isItemMenu = false;
            drawType = DrawType.brush;
            obj.Cursor = Cursors.Pen;
        }

        private void OnLineCommand(Canvas obj)
        {
            isItemMenu = false;
            drawType = DrawType.line;
            obj.Cursor = Cursors.Cross;
        }

        private void OnColorCommand(RibbonButton canvas)
        {
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            dlg.AllowFullOpen = true;
            dlg.ShowDialog();
            System.Windows.Media.Color color = new System.Windows.Media.Color();
            color.A = dlg.Color.A;
            color.R = dlg.Color.R;
            color.G = dlg.Color.G;
            color.B = dlg.Color.B;
            if (IsColor1)
                Color1 = new SolidColorBrush(color);
            else Color2 = new SolidColorBrush(color);
            if (IsCheckFill) ColorFill = Color1;
            else ColorFill = Color2;

        }

        private void OnChooseColor2Command(object obj)
        {
            IsColor1 = false;
        }

        private void OnChooseColor1Command(object obj)
        {
            IsColor1 = true;
        }

        private void OnStrokeCommand(RibbonButton canvas)
        {
            if (IsColor1)
            {
                Color1 = canvas.Background;
            }
            else
            {
                Color2 = canvas.Background;
            }
            if (IsCheckFill) ColorFill = Color1;
            else ColorFill = Color2;
        }

        private void OnStrokeThicknessCommand(RibbonMenuItem canvas)
        {
            isItemMenu = true;
            if (canvas.Header.Equals("1px"))
            {
                StrokeThickness = 1;
            }
            else if (canvas.Header.Equals("3px"))
            {
                StrokeThickness = 3;
            }
            else if (canvas.Header.Equals("5px"))
            {
                StrokeThickness = 5;
            }
            else if (canvas.Header.Equals("10px"))
            {
                StrokeThickness = 10;
            }
        }
        private DrawType drawType;
    }
}
