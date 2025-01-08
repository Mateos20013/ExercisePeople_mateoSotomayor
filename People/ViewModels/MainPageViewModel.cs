using People.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace People.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly PersonRepository _personRepo;

        public ObservableCollection<Person> People { get; set; }

        private string _newPersonName;
        public string NewPersonName
        {
            get => _newPersonName;
            set
            {
                _newPersonName = value;
                OnPropertyChanged(nameof(NewPersonName));
            }
        }

        public ICommand AddPersonCommand { get; }
        public ICommand DeletePersonCommand { get; }
        // Constructor sin parámetros
        public MainPageViewModel() : this(new PersonRepository())
        {
        }

        public MainPageViewModel(PersonRepository personRepo)
        {
            _personRepo = personRepo;
            People = new ObservableCollection<Person>();
            AddPersonCommand = new Command(async () => await AddPerson());
            DeletePersonCommand = new Command<Person>(async person => await DeletePerson(person));
            LoadPeople();
        }

        private async void LoadPeople()
        {
            var people = await _personRepo.GetAllPeople();
            People.Clear();
            foreach (var person in people)
                People.Add(person);
        }

        private async Task AddPerson()
        {
            if (!string.IsNullOrWhiteSpace(NewPersonName))
            {
                await _personRepo.AddNewPerson(NewPersonName);
                LoadPeople();
                NewPersonName = string.Empty;
            }
        }

        private async Task DeletePerson(Person person)
        {
            if (person != null)
            {
                await _personRepo.DeletePerson(person.Id);
                LoadPeople();
                await App.Current.MainPage.DisplayAlert(
                "Registro Eliminado",
                $"Mateo Sotomayor acaba de eliminar a {person.Name}.",
                "OK");
            }
        }
    }
}
