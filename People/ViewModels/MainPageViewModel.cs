using People.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace People.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly PersonRepository _repository;

        public ObservableCollection<Person> People { get; set; }
        public string NewPersonName { get; set; }
        public ICommand AddPersonCommand { get; }
        public ICommand DeletePersonCommand { get; }

        public MainPageViewModel(string dbPath)
        {
            _repository = new PersonRepository(dbPath);
            People = new ObservableCollection<Person>();
            AddPersonCommand = new Command(async () => await AddPerson());
            DeletePersonCommand = new Command<Person>(async (person) => await DeletePerson(person));
            LoadPeople();
        }

        private async void LoadPeople()
        {
            var people = await _repository.GetAllPeople();
            People.Clear();
            foreach (var person in people)
            {
                People.Add(person);
            }
        }

        private async Task AddPerson()
        {
            if (string.IsNullOrEmpty(NewPersonName))
                return;

            await _repository.AddNewPerson(NewPersonName);
            LoadPeople();
            NewPersonName = string.Empty;
            OnPropertyChanged(nameof(NewPersonName));
        }

        private async Task DeletePerson(Person person)
        {
            if (person == null)
                return;

            await _repository.DeletePerson(person);
            LoadPeople();
            await Application.Current.MainPage.DisplayAlert(
                "Registro Eliminado",
                $"{person.Name} acaba de eliminar un registro.",
                "OK");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
