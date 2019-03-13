# DNS Updater

A tiny background docker containerized process which automatically check your public ip and update the dns zone on the Cloudflare platform.

- It has been only tested and run on raspberry pi 2 & 3.
- It make a GET request to http://ipv4bot.whatismyipaddress.com to retrieve the public ip address.
- It connect & update your dns zone on cloudflare only when the IP change.

## Getting Started

Docker Create Service Command :

```
docker service create \
     --name dns-updater \
     --replicas 1 \
     --secret source=dns_updater_key,target=dns_updater_key \
     --secret source=dns_updater_zone,target=dns_updater_zone \
     -e DNS_UPDATER_EMAIL="mycloudflareaccount@me.com" \
     -e DNS_UPDATER_KEY_FILE="/run/secrets/dns_updater_key" \
     -e DNS_UPDATER_ZONE_FILE="/run/secrets/dns_updater_zone" \
     dungtri/docker-cloudflare-dns-updater:arm32
```

Docker Compose File :

```
version: "3.1"

services:
  dnsupdater:
    container_name: dns-updater
    image: dungtri/docker-cloudflare-dns-updater
    environment:
      DNS_UPDATER_EMAIL: mycloudflareaccount@me.com
      DNS_UPDATER_KEY_FILE: /run/secrets/dns_updater_key
      DNS_UPDATER_ZONE_FILE: /run/secrets/dns_updater_zone
    
    # More simple but less secure if you prefer to set the key & the zone directly 
    # as text plain, you can use these environment variables:
    # DNS_UPDATER_KEY: <token>
    # DNS_UPDATER_ZONE: <token>
    
    # (Optional) Delay between checks current ip & update when necessary (default: 30000).
    # SCHEDULER_CHECK_DELAY: <custom delay>

    secrets:
      - dns_updater_key
      - dns_updater_zone

secrets:
  dns_updater_key:
    external: true
  dns_updater_zone:
    external: true
```

Deploying the Service :

```
docker stack deploy -c dns-updater-arm32.yml dns-updater
```

The docker package are available on Docker Hub here: https://hub.docker.com/r/dungtri/docker-cloudflare-dns-updater

## Optional environment variable

* `SCHEDULER_CHECK_DELAY` Each time in millisecond, your current public internet protocol address is checked (default: 30000ms).

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/dungtri/docker-cloudflare-dns-updater/tags). 

## Authors

* **Dung Tri LE** - *Initial work* - [dungtri](https://github.com/dungtri)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
