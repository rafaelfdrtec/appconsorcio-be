Instruções para Junie (WebStorm) implementar consumo de API no Next.js

Contexto
- API ASP.NET Core com autenticação JWT e upload de arquivos (Azure Blob).
- Swagger disponível em /swagger em dev (veja e confirme os caminhos e esquemas).
- Base URL (configure no Next.js): defina NEXT_PUBLIC_API_URL no .env.local, por exemplo:
  NEXT_PUBLIC_API_URL=http://localhost:5152

Padrões gerais
- Autenticação: enviar Authorization: Bearer {token} nas rotas [Authorize].
- Content-Type:
  - JSON quando o DTO é de corpo simples.
  - multipart/form-data quando o controller usa [FromForm] ou há upload de arquivo (ex.: Arquivo).
- Datas retornam como strings "yyyy-MM-dd HH:mm:ss".

Como implementar no Next.js
1) Configurar cliente HTTP com Axios (recomendado)
- Arquivo: src/lib/api.ts
- Objetivo: injetar baseURL, Authorization automática e tratamento de 401.

Exemplo:
import axios from 'axios';

export const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
});

api.interceptors.request.use((config) => {
  if (typeof window !== 'undefined') {
    const token = localStorage.getItem('auth_token'); // ou cookies/NextAuth
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  }
  return config;
});

api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      // redirecionar para login, limpar token, etc.
    }
    return Promise.reject(err);
  }
);

2) Armazenamento do token
- Login via email/senha (/auth/login ou /api/usuarios/login): salve o token retornado no localStorage (auth_token) ou cookies httpOnly (mais seguro, porém requer rotas/app específicas).
- Login via Google (/auth/google): use o accessToken em response.tokens.accessToken.

3) Upload/Download
- Upload: use FormData e não defina manualmente o Content-Type (o browser define o boundary).
- Download: use responseType: 'blob' com Axios e crie um link para salvar.

Ex.: Upload de arquivo
const fd = new FormData();
fd.append('Arquivo', file);
await api.post(`/api/cartas/${cartaId}/anexos`, fd);

Ex.: Download
const res = await api.get(`/api/documentos/${id}/download`, { responseType: 'blob' });
const url = URL.createObjectURL(res.data);
const a = document.createElement('a');
a.href = url;
a.download = 'documento';
a.click();
URL.revokeObjectURL(url);

Lista de Endpoints por Recurso

Autenticação (sem auth)
Base: /auth

1) POST /auth/login
- Body (JSON): { "email": string, "password": string }
- Resposta: UsuarioRespostaDTO { id, nome, email, papel, ehVerificado, token }
- Uso: autenticação por email/senha; token no campo token.
- Frontend:
const { data } = await api.post('/auth/login', { email, password });
localStorage.setItem('auth_token', data.token);

2) POST /auth/google
- Body (JSON): { "idToken": string, "name"?: string, "email"?: string, "googleId"?: string, "avatar"?: string }
- Resposta: AuthResponseDTO { user: { id, nome, email, papel, ehVerificado, avatar? }, tokens: { accessToken, refreshToken? } }
- Uso: autenticação Google (NextAuth) via id_token; faz provisionamento automático de usuário.
- Frontend:
const { data } = await api.post('/auth/google', { idToken, name, email, googleId, avatar });
localStorage.setItem('auth_token', data.tokens.accessToken);

Usuários
Base: /api/usuarios

1) POST /api/usuarios/registrar
- Body (JSON): RegistroUsuarioDTO { nome, email, senha, papel }
- Resposta (201): UsuarioRespostaDTO { id, nome, email, papel, ehVerificado, token }
- Uso: criar usuário + retornar token.

2) POST /api/usuarios/login
- Body (JSON): LoginUsuarioDTO { email, senha }
- Resposta: UsuarioRespostaDTO
- Observação: duplicado conceitualmente com /auth/login; escolha um padrão no frontend.

3) GET /api/usuarios/{id} [Authorize]
- Path: id (int)
- Resposta: UsuarioRespostaDTO (Token vazio na consulta)
- Regra: apenas o próprio usuário ou admin pode acessar.

4) POST /api/usuarios/{id}/verificar [Authorize(Roles="admin")]
- Path: id (int)
- Resposta: UsuarioRespostaDTO (token vazio)
- Uso: marcar usuário como verificado.

Propostas
Base: /api/propostas (todas [Authorize])

1) POST /api/propostas
- Body: [FromForm] CriarPropostaDTO
  - Campos: CartaConsorcioId (int), Agio (decimal), PrazoMeses? (int)
- Resposta (201): PropostaRespostaDTO { id, cartaConsorcioId, compradorId, nomeComprador?, agio, prazoMeses?, status, motivoCancelamento?, criadaEm, canceladaEm?, efetivadaEm? }
- Frontend (FormData):
const fd = new FormData();
fd.append('CartaConsorcioId', String(cartaId));
fd.append('Agio', String(agio));
if (prazoMeses) fd.append('PrazoMeses', String(prazoMeses));
const { data } = await api.post('/api/propostas', fd);

2) GET /api/propostas/carta/{cartaId}
- Resposta: PropostaRespostaDTO[]
- Uso: listar propostas por carta.

3) POST /api/propostas/{id}/cancelar
- Body (JSON): { "motivo"?: string }
- Resposta: PropostaRespostaDTO
- Permissões: comprador, vendedor da carta ou admin.

4) POST /api/propostas/{id}/efetivar
- Body (JSON): { "valorVenda": number }
- Resposta: PropostaRespostaDTO
- Efeitos: marca proposta como efetivada, carta como vendida e seta valores de venda.

5) POST /api/propostas/{id}/anexos
- Body: multipart/form-data com campo Arquivo
- Resposta (201): AnexoRespostaDTO { id, nomeOriginal, contentType, tamanhoBytes, blobName, blobUrl, criadoEm }

Anexos de Carta
Base: /api/cartas/{cartaId}/anexos ([Authorize])

1) GET /api/cartas/{cartaId}/anexos
- Resposta: AnexoRespostaDTO[]

2) POST /api/cartas/{cartaId}/anexos
- Body: multipart/form-data com Arquivo
- Resposta (201): AnexoRespostaDTO
- Permissão: vendedor da carta ou admin.

Documentos de Usuário
Base: /api/documentos (todas [Authorize]; algumas admin-only)

1) POST /api/documentos/upload
- Body: multipart/form-data com:
  - Arquivo (file)
  - TipoDocumento (string)
- Resposta (201): DocumentoUsuarioRespostaDTO { id, usuarioId, nomeUsuario, tipoDocumento, nomeArquivo, contentType, tamanhoBytes, status, observacoesValidacao?, criadoEm, atualizadoEm, validadoPorId?, nomeValidador?, validadoEm? }
- Observações: substitui documento existente do mesmo tipo (deleta blob antigo).

2) GET /api/documentos/meus-documentos
- Resposta: DocumentoUsuarioRespostaDTO[]
- Uso: lista documentos do usuário autenticado.

3) GET /api/documentos/{id}
- Resposta: DocumentoUsuarioRespostaDTO
- Permissão: dono do documento ou admin.

4) GET /api/documentos/{id}/download
- Resposta: arquivo (stream); usar blob no frontend.

5) GET /api/documentos/admin/todos [Authorize(Roles="admin")]
- Query opcional: status, tipoDocumento
- Resposta: DocumentoUsuarioRespostaDTO[]

6) PUT /api/documentos/{id}/validar [Authorize(Roles="admin")]
- Body (JSON): ValidarDocumentoDTO { status: string, observacoesValidacao?: string }
- Resposta: DocumentoUsuarioRespostaDTO

7) DELETE /api/documentos/{id}
- Resposta: 204 No Content
- Permissão: dono ou admin (apaga blob e registro).

Administradoras
Base: /api/administradoras ([Authorize])

1) GET /api/administradoras
- Resposta: AdministradoraRespostaDTO[] { id, nome, cnpj, telefone, email, status, createdAt, updatedAt }

2) GET /api/administradoras/{id}
- Resposta: AdministradoraRespostaDTO
- Permissão: somente admin (verificação no método).

3) POST /api/administradoras
- Body (JSON): CriarAdministradoraDTO { nome, cnpj, telefone, email, status }
- Resposta (201): AdministradoraRespostaDTO
- Permissão: admin (verificação no método).

4) PUT /api/administradoras/{id}
- Body (JSON): AtualizarAdministradoraDTO { nome, cnpj, telefone, email, status }
- Resposta: 204 No Content
- Permissão: admin.

5) DELETE /api/administradoras/{id}
- Resposta: 204 No Content
- Permissão: admin; bloqueia se houver cartas vinculadas.

Cartas (rotas prováveis)
Base provável: /api/cartas ([Authorize] em operações de escrita; leitura possivelmente aberta/logada)
Observação: O arquivo do controller contém os métodos abaixo; confira os caminhos no Swagger e ajuste caso haja diferenças.

1) POST /api/cartas
- Body (JSON): CriarCartaConsorcioDTO
- Resposta (201): CartaConsorcioRespostaDTO
- Uso: criar carta para venda (vendedor autenticado).

2) GET /api/cartas/disponiveis
- Resposta: CartaConsorcioRespostaDTO[]
- Uso: listar cartas com status disponíveis.

3) GET /api/cartas/{id}
- Resposta: CartaConsorcioRespostaDTO

4) GET /api/cartas/pesquisar?... (query)
- Query: PesquisarCartaConsorcioDTO (campos de filtro)
- Resposta: CartaConsorcioRespostaDTO[]

5) PUT /api/cartas/{id}/status
- Body (JSON): AtualizarStatusCartaDTO { novoStatus, motivo? }
- Resposta: CartaConsorcioRespostaDTO
- Regra: valida transição de status.

6) POST /api/cartas/{id}/verificar
- Resposta: CartaConsorcioRespostaDTO
- Permissão: provavelmente admin (confira no Swagger).

Exemplos de serviços no Next.js

1) Auth service
export const authService = {
  async loginEmailSenha(email: string, password: string) {
    const { data } = await api.post('/auth/login', { email, password });
    localStorage.setItem('auth_token', data.token);
    return data;
  },
  async loginGoogle(idToken: string, extra?: { name?: string; email?: string; googleId?: string; avatar?: string }) {
    const { data } = await api.post('/auth/google', { idToken, ...extra });
    localStorage.setItem('auth_token', data.tokens.accessToken);
    return data;
  },
};

2) Propostas service
export const propostasService = {
  async criar(cartaId: number, agio: number, prazoMeses?: number) {
    const fd = new FormData();
    fd.append('CartaConsorcioId', String(cartaId));
    fd.append('Agio', String(agio));
    if (prazoMeses) fd.append('PrazoMeses', String(prazoMeses));
    const { data } = await api.post('/api/propostas', fd);
    return data;
  },
  async listarPorCarta(cartaId: number) {
    const { data } = await api.get(`/api/propostas/carta/${cartaId}`);
    return data;
  },
  async cancelar(id: number, motivo?: string) {
    const { data } = await api.post(`/api/propostas/${id}/cancelar`, { motivo });
    return data;
  },
  async efetivar(id: number, valorVenda: number) {
    const { data } = await api.post(`/api/propostas/${id}/efetivar`, { valorVenda });
    return data;
  },
  async uploadAnexo(id: number, file: File) {
    const fd = new FormData();
    fd.append('Arquivo', file);
    const { data } = await api.post(`/api/propostas/${id}/anexos`, fd);
    return data;
  },
};

3) Documentos service
export const documentosService = {
  async upload(tipoDocumento: string, file: File) {
    const fd = new FormData();
    fd.append('TipoDocumento', tipoDocumento);
    fd.append('Arquivo', file);
    const { data } = await api.post('/api/documentos/upload', fd);
    return data;
  },
  async meusDocumentos() {
    const { data } = await api.get('/api/documentos/meus-documentos');
    return data;
  },
  async obter(id: string) {
    const { data } = await api.get(`/api/documentos/${id}`);
    return data;
  },
  async download(id: string) {
    const res = await api.get(`/api/documentos/${id}/download`, { responseType: 'blob' });
    return res.data;
  },
  async listarTodosAdmin(params?: { status?: string; tipoDocumento?: string }) {
    const { data } = await api.get('/api/documentos/admin/todos', { params });
    return data;
  },
  async validar(id: string, payload: { status: string; observacoesValidacao?: string }) {
    const { data } = await api.put(`/api/documentos/${id}/validar`, payload);
    return data;
  },
  async excluir(id: string) {
    await api.delete(`/api/documentos/${id}`);
  },
};

4) Anexos de Carta service
export const cartaAnexosService = {
  async listar(cartaId: number) {
    const { data } = await api.get(`/api/cartas/${cartaId}/anexos`);
    return data;
  },
  async upload(cartaId: number, file: File) {
    const fd = new FormData();
    fd.append('Arquivo', file);
    const { data } = await api.post(`/api/cartas/${cartaId}/anexos`, fd);
    return data;
  },
};

5) Administradoras service
export const administradorasService = {
  async listar() {
    const { data } = await api.get('/api/administradoras');
    return data;
  },
  async obter(id: string) {
    const { data } = await api.get(`/api/administradoras/${id}`);
    return data;
  },
  async criar(payload: { nome: string; cnpj: string; telefone?: string; email?: string; status: string }) {
    const { data } = await api.post('/api/administradoras', payload);
    return data;
  },
  async atualizar(id: string, payload: { nome: string; cnpj: string; telefone?: string; email?: string; status: string }) {
    await api.put(`/api/administradoras/${id}`, payload);
  },
  async excluir(id: string) {
    await api.delete(`/api/administradoras/${id}`);
  },
};

6) Usuários service
export const usuariosService = {
  async registrar(payload: { nome: string; email: string; senha: string; papel: string }) {
    const { data } = await api.post('/api/usuarios/registrar', payload);
    localStorage.setItem('auth_token', data.token);
    return data;
  },
  async login(payload: { email: string; senha: string }) {
    const { data } = await api.post('/api/usuarios/login', payload);
    localStorage.setItem('auth_token', data.token);
    return data;
  },
  async obter(id: number) {
    const { data } = await api.get(`/api/usuarios/${id}`);
    return data;
  },
  async verificar(id: number) {
    const { data } = await api.post(`/api/usuarios/${id}/verificar`);
    return data;
  },
};

Observações importantes para a Junie
- Sempre ler NEXT_PUBLIC_API_URL do env.
- Incluir Authorization automaticamente via interceptor quando houver token.
- Para multipart, usar FormData e não setar manualmente Content-Type.
- Tratar 401: redirecionar para login/reauth.
- Usar os tipos dos DTOs no frontend para tipar as respostas (crie interfaces espelhando os DTOs da API).
- Validar e ajustar os endpoints de Cartas no Swagger (alguns caminhos podem variar: p.ex. /api/cartas/pesquisar ou /api/cartas?param=...). Atualize o serviço se necessário.

Checklist de implementação
- [ ] Criar src/lib/api.ts (Axios com interceptors).
- [ ] Criar serviços por domínio: authService, usuariosService, propostasService, documentosService, cartaAnexosService, administradorasService, cartasService (após confirmar rotas de cartas).
- [ ] Criar hooks e actions conforme telas.
- [ ] Configurar persistência do token (localStorage/cookie) e logout.
- [ ] Testar upload e download (tipos: pdf, jpg, jpeg, png; máx. 10MB).
- [ ] Confirmar no Swagger os caminhos exatos dos endpoints de Cartas e ajustar se divergirem.

Como validar rapidamente
- Acesse /swagger para ver os paths e schemas.
- Faça smoke-test com Postman/cURL para login e uma rota protegida.
- No Next.js, teste um fluxo: login -> listar administradoras -> criar proposta -> anexar arquivo -> baixar documento.
