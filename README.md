# Configurando a aplicação com o Prometheus e Grafana

Essa configuração se baseia na premissa de que você está utilizando uma máquina windows e já possui o WSL instalado e configurado, juntamente ao Docker.

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


# Adendos

## Problema do container não enxergar sua aplicação

Ao executar a aplicação em .net na máquina windows e o prometheus estar em um container no docker, foi necessário adicionar as linhas de código abaixo:

``` csharp
builder.WebHost.UseUrls("http://0.0.0.0:5260");
```

Sendo "5260" a porta da minha aplicação.
