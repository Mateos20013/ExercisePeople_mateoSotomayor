namespace People;
using SQLite;
using People.Models;
using System.Threading.Tasks;

public class PersonRepository
{
    string _dbPath;

    public string StatusMessage { get; set; }

    // TODO: Add variable for the SQLite connection
    private SQLiteAsyncConnection conn;

    private async Task Init()
    {
        if (conn != null)
            return;

        conn = new SQLiteAsyncConnection(_dbPath);

        await conn.CreateTableAsync<Person>();
    }

    public PersonRepository(string dbPath)
    {
        _dbPath = dbPath;
    }

    public PersonRepository()
    {
    }

    public async Task AddNewPerson(string name)
    {
        int result = 0;
        try
        {
            // Call Init()
            await Init();

            // basic validation to ensure a name was entered
            if (string.IsNullOrEmpty(name))
                throw new Exception("Valid name required");

            result = await conn.InsertAsync(new Person { Name = name });

            StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, name);
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to add {0}. Error: {1}", name, ex.Message);
        }
    }

    public async Task<List<Person>> GetAllPeople()
    {
        try
        {
            await Init();
            return await conn.Table<Person>().ToListAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
        }

        return new List<Person>();
    }
    public async Task<Person> GetPersonById(int id)
    {
        try
        {
            await Init();
            return await conn.FindAsync<Person>(id);
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to retrieve person. Error: {0}", ex.Message);
            return null;
        }
    }

    public async Task DeletePerson(int id)
    {
        try
        {
            await Init();
            var person = await conn.FindAsync<Person>(id);
            if (person != null)
            {
                await conn.DeleteAsync(person);
                StatusMessage = $"Person with ID {id} deleted successfully.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to delete person. Error: {0}", ex.Message);
        }
    }

}