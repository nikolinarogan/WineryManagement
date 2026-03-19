

export interface Sirovovino {
  idsirvina: number
  nazivsirvina: string
  kolicinasirvina: number
  kvalitet: string
  godproizvodnje: number
  poreklo: PorekloSirovina[]
}

export interface PorekloSirovina {
  ubranasirovinaId: number
  berbaNaziv: string
  parcelaNaziv: string
  sortaNaziv: string | null
  brojTretmana: number
  kolicinaGrozdja: number  // kg grožđa utrošeno iz ove ubrane sirovine
}

export interface UbranaSirovinaInput {
  ubranasirovinaId: number
  kolicinaGrozdja: number  // kg grožđa koje se koristi
}

export interface CreateSirovovinoRequest {
  nazivsirvina: string
  kolicinasirvina: number       // Litara vina
  kvalitet: string
  godproizvodnje: number
  ubraneSirovine: UbranaSirovinaInput[]  // Lista ubranih sirovina sa količinama grožđa
}

export interface UpdateSirovovinoRequest {
  nazivsirvina: string
  kolicinasirvina: number
  kvalitet: string
}

