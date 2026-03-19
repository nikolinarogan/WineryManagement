import api from './api'
import type {
  BocaInventar,
  Boca,
  CreateBocaPunjenjeRequest,
  BocePunjenjeResult
} from '@/types/boca'

class BocaService {
  async punjenjeBoca(data: CreateBocaPunjenjeRequest): Promise<BocePunjenjeResult> {
    const response = await api.post('/boca/punjenje', data)
    return response.data
  }

  async getAllBoce(): Promise<BocaInventar[]> {
    const response = await api.get('/boca')
    return response.data
  }

  async getBoceByMagacin(magacinId: number): Promise<Boca[]> {
    const response = await api.get(`/boca/magacin/${magacinId}`)
    return response.data
  }

  async getBrojBocaUMagacinu(magacinId: number): Promise<number> {
    const response = await api.get(`/boca/magacin/${magacinId}/broj`)
    return response.data.brojBoca
  }
}

export default new BocaService()

