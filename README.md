## Integrantes

- **Caio Alexandre dos Santos** - RM: 558460
- **Leandro do Nascimento Souza** - RM: 558893
- **Rafael de Mônaco Maniezo** - RM: 556079
- **Vinicius Rozas Pannuci de Paula Cont** - RM: 555338

# 🛰️ Arandu — Sistema Aeroespacial do Agronegócio

**Arandu** (do tupi: *conhecimento*, *sabedoria*) é um sistema de monitoramento de lavouras via satélite desenvolvido em C# (.NET 8), que processa dados espectrais de satélites agrícolas para diagnosticar o estado das culturas brasileiras em tempo real.

---

## 🌱 Motivação

O agronegócio brasileiro responde por cerca de 25% do PIB nacional e opera em escala continental. Monitorar a saúde das lavouras manualmente é inviável nessa dimensão — é aqui que a tecnologia espacial entra.

O **índice NDVI** (Normalized Difference Vegetation Index) é calculado a partir das bandas espectrais captadas por satélites ópticos. Ele mede a densidade e saúde da vegetação com base na diferença entre a reflectância no infravermelho próximo (NIR) e no vermelho (Red):

```
NDVI = (NIR - Red) / (NIR + Red)
```

| NDVI | Diagnóstico |
|------|-------------|
| < 0.2 | 🔴 CRÍTICO — solo exposto ou degradação severa |
| 0.2 – 0.5 | 🟡 ATENÇÃO — estágio inicial ou estresse hídrico |
| > 0.5 | 🟢 OK — lavoura saudável com alta biomassa |

O Arandu automatiza esse diagnóstico, permitindo que agrônomos e gestores identifiquem rapidamente setores críticos entre milhares de capturas de satélite.

---

## 🏗️ Arquitetura e Integração

O sistema segue uma arquitetura em camadas com **Inversão de Dependência** aplicada em todos os pontos de extensão:

```
Program.cs
    │
    ├── IDataLoader ──────────► CsvProcessor
    │       └── lê sat_data.csv e popula o repositório
    │
    ├── IDataRepository ──────► DataRepository
    │       └── armazena e consulta os dados em memória
    │
    └── IAlertService ────────► ConsoleAlertService
            └── emite alertas coloridos por nível de diagnóstico
```

### Como os componentes se integram

1. **Inicialização**: o container `Microsoft.Extensions.DependencyInjection` registra as três interfaces e resolve as dependências automaticamente.
2. **Carregamento**: `CsvProcessor` lê o arquivo `sat_data.csv`, valida cada linha individualmente (linhas corrompidas são descartadas com aviso sem derrubar o sistema) e instancia objetos `OpticalData` com as bandas espectrais brutas.
3. **Armazenamento**: `DataRepository` mantém os dados em memória e expõe consultas LINQ por satélite, região e intervalo de datas.
4. **Análise**: `OpticalData.AnalyzeCropState()` calcula o NDVI dinamicamente e retorna o diagnóstico via polimorfismo.
5. **Exibição**: `ConsoleAlertService` emite cada resultado com cor correspondente ao nível de alerta.

### Extensibilidade

Para **substituir o canal de saída** (ex.: enviar alertas por e-mail), basta criar uma nova classe que implemente `IAlertService` e trocar o registro no container — nenhuma outra classe precisa ser alterada.

Para **substituir a fonte de dados** (ex.: banco de dados ou API REST), basta implementar `IDataLoader` e `IDataRepository` com a nova fonte.

---

## 📁 Estrutura de Pastas

```
Monitoramento Aeroespacial Agro/
│
├── Data/
│   └── sat_data.csv                  # Dataset de capturas dos satélites
│
├── Exceptions/
│   └── SpaceDataException.cs         # Exceção de domínio do sistema
│
├── Interfaces/
│   ├── IAlertService.cs              # Contrato para emissão de alertas
│   ├── IDataLoader.cs                # Contrato para carregamento de dados
│   └── IDataRepository.cs            # Contrato para repositório de dados
│
├── Models/
│   ├── GeoCoordinate.cs              # Struct de coordenadas geográficas
│   ├── SatelliteData.cs              # Classe abstrata base
│   ├── OpticalData.cs                # Partial 1: propriedades e construtor
│   ├── OpticalData.Analysis.cs       # Partial 2: classificação pelo NDVI
│   └── OpticalData.Reports.cs        # Partial 3: formatação e relatórios
│
├── Services/
│   ├── ConsoleAlertService.cs        # Alertas coloridos no console
│   ├── CsvProcessorService.cs        # Leitura e parse do CSV
│   └── DataRepository.cs             # Repositório em memória
│
└── Program.cs                        # Ponto de entrada e menu principal
```

## Fluxograma
<img width="1781" height="2260" alt="Projeto Arandu C# (3)" src="https://github.com/user-attachments/assets/5bb6a6cc-6269-45cf-95e4-377b3515b544" />

---

### Evidências
Tela inicial:
<img width="714" height="276" alt="image" src="https://github.com/user-attachments/assets/89307903-1d15-42e7-8d5f-b1b314e07d33" />

Tela de relatório geral:
<img width="1754" height="664" alt="image" src="https://github.com/user-attachments/assets/8bc2d419-552a-4271-abfb-d32e4ba00188" />

Tela de consulta por satélite:
<img width="1734" height="198" alt="image" src="https://github.com/user-attachments/assets/11eafa22-7497-449a-a045-7fffdb05d2d9" />

Tela de consulta por região:
<img width="1749" height="256" alt="image" src="https://github.com/user-attachments/assets/1c820425-3407-4eb5-9ab2-04063ccf6eb6" />

Tela de consulta por intervalo de datas:
<img width="1761" height="323" alt="image" src="https://github.com/user-attachments/assets/9e87c85f-ec00-4d0c-af20-cb3d353f1c8f" />

---

## 🧩 Conceitos de C# Aplicados

| Conceito | Onde é aplicado |
|----------|----------------|
| **Classe abstrata** | `SatelliteData` — não pode ser instanciada diretamente |
| **Herança e polimorfismo** | `OpticalData` herda de `SatelliteData` e sobrescreve `AnalyzeCropState()` |
| **Classe estática** | `NdviThresholds` — centraliza as constantes de limiar do NDVI |
| **Classe `partial`** | `OpticalData` dividida em 3 partes: propriedades, análise e relatórios |
| **Struct** | `GeoCoordinate` — tipo de valor imutável com validação interna de coordenadas |
| **Interfaces** | `IAlertService`, `IDataLoader`, `IDataRepository` |
| **Injeção de dependência** | Container `ServiceCollection` registra e resolve todas as dependências |
| **LINQ** | Filtros por satélite, região e data no `DataRepository`; `GroupBy` nas listagens |
| **Tratamento de exceções** | `SpaceDataException` lançada e capturada em múltiplos pontos críticos |
| **DateTime preciso** | `TryParseExact`, filtro por intervalo com `AddSeconds(-1)`, formatação localizada |
| **Métodos estáticos privados** | `ClassifyByNdvi()`, `ResolveColor()`, `ResolverEscolha()`, `ResolverStatusCor()` |

---

## ▶️ Como Executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022+ ou VS Code com extensão C#

### Passos

```bash
# Clone o repositório
git clone https://github.com/caio-alex/Projeto-Arandu-Csharp.git

# Entre na pasta do projeto
cd "Monitoramento Aeroespacial Agro"

# Restaure as dependências
dotnet restore

# Execute
dotnet run
```

O arquivo `sat_data.csv` é copiado automaticamente para a pasta de saída durante o build.

---

## 📡 Formato do Dataset

O arquivo `Data/sat_data.csv` segue o formato:

```
SatelliteId,Regiao,Latitude,Longitude,CaptureDate,RedBand,NirBand
SAT-MT-01,Mato Grosso,-12.6412,-55.3492,2026-05-26 06:00:00,0.06,0.82
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `SatelliteId` | string | Identificador único do satélite |
| `Regiao` | string | Estado ou região monitorada |
| `Latitude` | double | Entre -90 e 90 |
| `Longitude` | double | Entre -180 e 180 |
| `CaptureDate` | DateTime | Data e hora da captura |
| `RedBand` | double | Reflectância na banda do vermelho |
| `NirBand` | double | Reflectância no infravermelho próximo |

O NDVI **não é armazenado no CSV** — é calculado dinamicamente pela propriedade `OpticalData.NDVI` a partir das bandas brutas, garantindo que a regra de negócio viva no código, não nos dados.

---

## 🛰️ Satélites Monitorados

| Satélite | Região | Estado |
|----------|--------|--------|
| SAT-MT-01 | Mato Grosso | MT |
| SAT-GO-02 | Goiás | GO |
| SAT-PR-03 | Paraná | PR |
| SAT-MS-04 | Mato Grosso do Sul | MS |
| SAT-BA-05 | Bahia (MATOPIBA) | BA |
| SAT-RS-06 | Rio Grande do Sul | RS |

---

## 📦 Dependências

| Pacote | Versão | Uso |
|--------|--------|-----|
| `Microsoft.Extensions.DependencyInjection` | 10.0.8 | Container de injeção de dependência |
