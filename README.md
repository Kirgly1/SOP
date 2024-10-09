Пример добавление полувагона с состоянием погрузки
{
  "id": 1,
  "cargo": "Sand",
  "capacity": 100,
  "loaded": 50,
  "isLoaded": false
}

Пример выполнения запроса на получение всех вагонов в GraphQL
query {
  wagons {
    cargo
    capacity
    loaded
    isLoaded
  }
}

Пример выполнения мутации вагона в GraphQL
mutation {
  createWagon(input: { cargo: "Coal", capacity: 10, loaded: 0, isLoaded: false }) {
    objectId
    cargo
    capacity
    loaded
    isLoaded
  }
}

