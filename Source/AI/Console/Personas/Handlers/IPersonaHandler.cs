﻿namespace AI.Sample.Personas.Handlers;

public interface IPersonaHandler {
    PersonaEntity[] List();
    PersonaEntity? GetByKey(uint key);
    PersonaEntity? GetByName(string name);
    PersonaEntity Create(Action<PersonaEntity> setUp);
    void Add(PersonaEntity persona);
    void Update(PersonaEntity persona);
    void Remove(uint key);
}