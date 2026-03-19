export interface LoginRequest {
  email: string
  sifra: string
}

export interface RegisterRequest {
  ime: string
  prez: string
  jmbg: string
  email: string
  kategorija: 'Enolog' | 'Somleijer' | 'Radnik'
  privremenaLozinka: string
  // Dodatna polja za svaki tip
  brsert?: string          // Za Enologa
  fizickaspremnost?: string // Za Radnika
  specijalnost?: string     // Za Somleijera
}

export interface AuthResponse {
  token: string
  email: string
  ime: string
  prez: string
  kategorija: string
  idzap: number
  expiresAt: string
}

export interface CurrentUser {
  idzap: number
  email: string
  ime: string
  prez: string
  kategorija: string
}

export interface ChangePasswordRequest {
  staraLozinka: string
  novaLozinka: string
  potvrdaLozinke: string
}

export interface Profile {
  idzap: number
  ime: string
  prez: string
  jmbg: string
  email: string
  kategorija: string
  // Dodatna polja za subtype-ove
  brsert?: string            // Enolog
  bonusucinak?: number        // Menadžer
  fizickaspremnost?: string   // Radnik
  specijalnost?: string       // Somleijer
}

export interface Employee {
  idzap: number
  ime: string
  prez: string
  email: string
  jmbg: string
  kategorija: string
}

