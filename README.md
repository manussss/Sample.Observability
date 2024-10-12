# Configurando a aplicação com Prometheus e Grafana

Essa configuração se baseia nas seguintes premissas:
- Você está utilizando uma máquina windows
- Possui o WSL instalado e configurado
- Possui o Docker instalado e configurado
- Possui uma conta no Grafana (incluso conta free-cloud)

Siga os passos abaixo no seu WSL.

## Criar uma network no Docker

Crie uma rede no docker para que os containers compartilhem da mesma.

`docker network create monitoring`

## Configurar o Prometheus

### Criar e navegar a um diretório

`mkdir ~/prometheus`

`cd ~/prometheus`

### Crie o arquivo prometheus.yml

``` yaml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: 'dotnet_app'
    metrics_path: '/metrics'
    static_configs:
      - targets: ['111.111.1.11:5000']  # Substitua pelo seu IP e porta da aplicação
```

Foi necessário colocar o meu IPV4, obtido através do comando `ipconfig`.

### Executar container do Prometheus

``` bash
docker run -d --name prometheus \
  --network monitoring \
  -p 9090:9090 \
  -v $(pwd)/prometheus.yml:/etc/prometheus/prometheus.yml \
  prom/prometheus
```

## Acesso ao Prometheus

Você pode acessar a GUI do Prometheus no endereço e porta configurados, neste caso:

`http://localhost:9090/`

### Targets

Quando sua aplicação subir, você pode validar que o Prometheus consegue obter suas métricas acessando:

`http://localhost:9090/targets?search=`

![image](https://github.com/user-attachments/assets/8b6c4e01-b4df-432a-ac44-7eda738117cf)

## Configurar Grafana & Prometheus

1. Acesse sua conta Grafana
2. Acesse `Connections`
3. `Private data source connect`
4. `Add new network`
5. Defina um nome
6. Selecione a configuração de sua preferência, neste caso, `Docker`
7. Gere um novo token
8. Execute a imagem do Docker gerada com seu token
9. Teste a conexão do agente

![image](https://github.com/user-attachments/assets/bfdf0098-6c16-4f64-8191-36f4f3441510)

10. Clique em `create a new data source`
11. Selecione `Prometheus`
12. Em `Connection`, defina: `http://id-container:9090`
13. Em `HTTP Method` selecione `GET`
14. Em `Private data source connect` selecione sua conexão

![image](https://github.com/user-attachments/assets/67f8bb50-d2ac-4d7c-8e7f-07e4c6aa5ff0)

15. Clique em `Save & Test`

Com estes passos, é possível visualizar as métricas da aplicação em `Explore`:

![image](https://github.com/user-attachments/assets/f8003b58-0f9c-442e-a122-af4cd0a99a58)


# Problemas conhecidos

## Container do Prometheus não consegue enxergar sua aplicação

Ao executar a aplicação em .net na máquina windows e o prometheus estar em um container no docker, foi necessário adicionar as linhas de código abaixo:

``` csharp
builder.WebHost.UseUrls("http://0.0.0.0:5260");
```

Sendo "5260" a porta da minha aplicação.
