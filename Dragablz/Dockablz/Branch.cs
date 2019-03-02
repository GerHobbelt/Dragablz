using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dragablz.Dockablz
{
  [TemplatePart(Name = FIRST_CONTENT_PRESENTER_PART_NAME, Type = typeof(ContentPresenter))]
  [TemplatePart(Name = SECOND_CONTENT_PRESENTER_PART_NAME, Type = typeof(ContentPresenter))]
  public class Branch : Control
  {
    private const string FIRST_CONTENT_PRESENTER_PART_NAME = "PART_FirstContentPresenter";
    private const string SECOND_CONTENT_PRESENTER_PART_NAME = "PART_SecondContentPresenter";

    static Branch()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Branch), new FrameworkPropertyMetadata(typeof(Branch)));
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
      nameof(Orientation), typeof(Orientation), typeof(Branch), new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
      get => (Orientation)GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty FirstItemProperty = DependencyProperty.Register(
      nameof(FirstItem), typeof(object), typeof(Branch), new PropertyMetadata(default(object)));

    public object FirstItem
    {
      get => GetValue(FirstItemProperty);
      set => SetValue(FirstItemProperty, value);
    }

    public static readonly DependencyProperty FirstItemLengthProperty = DependencyProperty.Register(
      nameof(FirstItemLength), typeof(GridLength), typeof(Branch), new FrameworkPropertyMetadata(new GridLength(0.49999, GridUnitType.Star), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public GridLength FirstItemLength
    {
      get => (GridLength)GetValue(FirstItemLengthProperty);
      set => SetValue(FirstItemLengthProperty, value);
    }

    public static readonly DependencyProperty SecondItemProperty = DependencyProperty.Register(
      nameof(SecondItem), typeof(object), typeof(Branch), new PropertyMetadata(default(object)));

    public object SecondItem
    {
      get => GetValue(SecondItemProperty);
      set => SetValue(SecondItemProperty, value);
    }

    public static readonly DependencyProperty SecondItemLengthProperty = DependencyProperty.Register(
      nameof(SecondItemLength), typeof(GridLength), typeof(Branch), new FrameworkPropertyMetadata(new GridLength(0.50001, GridUnitType.Star), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public GridLength SecondItemLength
    {
      get => (GridLength)GetValue(SecondItemLengthProperty);
      set => SetValue(SecondItemLengthProperty, value);
    }

    /// <summary>
    /// Gets the proportional size of the first item, between 0 and 1, where 1 would represent the entire size of the branch.
    /// </summary>
    /// <returns></returns>
    public double GetFirstProportion()
      => (1 / (FirstItemLength.Value + SecondItemLength.Value)) * FirstItemLength.Value;

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      FirstContentPresenter = GetTemplateChild(FIRST_CONTENT_PRESENTER_PART_NAME) as ContentPresenter;
      SecondContentPresenter = GetTemplateChild(SECOND_CONTENT_PRESENTER_PART_NAME) as ContentPresenter;
    }

    internal ContentPresenter FirstContentPresenter { get; private set; }
    internal ContentPresenter SecondContentPresenter { get; private set; }
  }
}
