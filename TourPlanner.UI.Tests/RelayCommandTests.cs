
namespace TourPlanner.UI.Tests
{
    using Xunit;
    using TourPlanner.UI.Commands;

    public class RelayCommandTests
    {
        [Fact]
        public void RelayCommand_CanExecute_ReturnsTrue() {
            RelayCommand cmd = new RelayCommand(o => { });
            Assert.True(cmd.CanExecute(null));
        }
    }
}
