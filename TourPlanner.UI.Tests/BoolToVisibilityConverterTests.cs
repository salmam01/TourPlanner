namespace TourPlanner.UI.Tests
{
    using Xunit;
    using TourPlanner.UI.Converters;
    using System.Windows;

    public class BoolToVisibilityConverterTests
    {
        [Fact]
        public void Convert_True_ReturnsVisible() {
            BoolToVisibilityConverter converter = new BoolToVisibilityConverter();
            object? result = converter.Convert(true, null, null, null);
            Assert.Equal(Visibility.Visible, result);
        }

        [Fact]
        public void Convert_False_ReturnsCollapsed() {
            var converter = new BoolToVisibilityConverter();
            object? result = converter.Convert(false, null, null, null);
            Assert.Equal(Visibility.Collapsed, result);
        }
    }
} 