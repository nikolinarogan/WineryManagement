export interface LagerovanoVino {
  sirovovinoIdsirvina: number
  nazivSirovogVina: string
  bureIdbur: number
  oznakaBureta: string
  materijalBureta: string
  zapreminaBureta: number
  nazivPodruma: string
  datpunjenja: string
  datpraznjenja: string
  jeAktivno: boolean
  brojDana: number
}

export interface DostupnoBure {
  idbur: number
  oznakabur: string
  materijal: string
  zapremina: number
  nazivPodruma: string
  jeZauzeto: boolean
}

export interface CreateLagerovanjeRequest {
  sirovovinoIdsirvina: number
  bureIdbur: number
  datpunjenja: string
}

export interface UpdateLagerovanjeRequest {
  datpraznjenja: string
}

