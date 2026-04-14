using System.Text.Json;
using System.Text.Json.Serialization;
using HeroesApi.Data;
using HeroesApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HeroesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HeroesController : ControllerBase {

    [HttpGet]
    public ActionResult<List<Hero>> GetAll() {
        return Ok(HeroesStore.Heroes);
    }
    
    [HttpGet("{id}")]
    public ActionResult<Hero> GetById(int id) {
        var hero = HeroesStore.Heroes.FirstOrDefault(h => h.Id == id);
        if (hero is null) {
            return NotFound(new { message = $"Герой с id={id} не найден" });
        }
        return Ok(hero);
    }

    [HttpGet("demo")]
    public ActionResult GetDemo() {
        var hero = HeroesStore.Heroes.FirstOrDefault();
        if (hero is null) {
            return NotFound(new { message = "Список героев пуст" });
        }

        var defaultOptions = new JsonSerializerOptions {
            WriteIndented = true
        };

        var ourOptions = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        return Ok(new {
            withDefaults = JsonSerializer.Deserialize<object>(
                JsonSerializer.Serialize(hero, defaultOptions), defaultOptions),
            withOurSettings = JsonSerializer.Deserialize<object>(
                JsonSerializer.Serialize(hero, ourOptions), ourOptions),
            note = "Сравните имена полей и значение universe в двух вариантах"
        });
    }
    [HttpGet("serialize")]
    public ActionResult GetSerialize() {
        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        var hero = new Hero {
            Id = 99,
            Name = "Тестовый герой",
            RealName = "Студент",
            Universe = Universe.Marvel,
            PowerLevel = 50,
            Powers = new() { "Программирование", "дебаггинг" },
            Weapon = new() { Name = "Клавиатура", IsRanged = false },
            InternalNotes = "Это поле не попадёт в JSON"
        };

        string serialized = JsonSerializer.Serialize(hero, options);
        var deserialized = JsonSerializer.Deserialize<Hero>(serialized, options);

        return Ok(new {
            serializedJson = serialized,
            deserializedObject = deserialized,
internalNotesAfterDeserialize = deserialized?.InternalNotes ?? "null - поле было проигнорировано"
        });
    }
}