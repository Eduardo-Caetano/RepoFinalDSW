import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5022/api',
})

export async function getAtuacaoDeputado(id, ano) {
  const response = await api.get(`/dashboard/atuacao/${id}`, {
    params: { ano },
  })
  return response.data
}
