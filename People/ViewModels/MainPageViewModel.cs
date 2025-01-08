using People.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace People.ViewModels;

public class MainPageViewModel : BaseViewModel
{
    public ObservableCollection<Person> PeopleList { get; set; } = new ObservableCollection<Person>();
    private readonly PersonRepository _repository;

    public Command AddPersonCommand { get; }
    public Command<Person> DeletePersonCommand { get; }

    private string _newPersonName;
    public string NewPersonName
    {
        get => _newPersonName;
        set => SetProperty(ref _newPersonName, value); // Usa SetProperty del BaseViewModel
    }

    // Constructor con parámetro (usado por MauiProgram.cs)
    public MainPageViewModel(string dbPath)
    {
        _repository = new PersonRepository(dbPath);

        AddPersonCommand = new Command(async () => await AddPerson());
        DeletePersonCommand = new Command<Person>(async (person) => await DeletePerson(person));
    }

    // Constructor predeterminado (necesario para soporte XAML)
    public MainPageViewModel() : this(Path.Combine(FileSystem.AppDataDirectory, "people.db3"))
    {
    }

    // Cargar la lista de personas
    public async Task LoadPeople()
    {
        var people = await _repository.GetAllPeople();
        PeopleList.Clear();
        foreach (var person in people)
        {
            PeopleList.Add(person);
        }
    }

    // Agregar una nueva persona
    private async Task AddPerson()
    {
        if (string.IsNullOrWhiteSpace(NewPersonName))
        {
            await App.Current.MainPage.DisplayAlert("Error", "El nombre no puede estar vacío.", "OK");
            return;
        }

        try
        {
            await _repository.AddNewPerson(NewPersonName);
            NewPersonName = string.Empty;
            await LoadPeople();
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", $"No se pudo agregar la persona: {ex.Message}", "OK");
        }
    }

    // Eliminar una persona
    private async Task DeletePerson(Person person)
    {
        if (person == null)
            return;

        bool isConfirmed = await App.Current.MainPage.DisplayAlert(
            "Confirmar Eliminación",
            $"¿Estás seguro de que deseas eliminar a {person.Name}?",
            "Sí",
            "No");

        if (isConfirmed)
        {
            try
            {
                await _repository.DeletePerson(person);
                await LoadPeople();

                // Mensaje de eliminación
                await App.Current.MainPage.DisplayAlert(
                    "Registro Eliminado",
                    $"Mateo Sotomayor acaba de eliminar a {person.Name}.",
                    "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar a la persona: {ex.Message}", "OK");
            }
        }
    }
}
