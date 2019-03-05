# DNS Updater

A tiny background docker containerized process which automatically check your public ip and update the dns zone on the Cloudflare platform.

- It has been only tested and run on raspberry pi 2 & 3.
- It make a GET request to http://ipv4bot.whatismyipaddress.com to retrieve the public ip address.
- It connect & update your dns zone only when the IP change on cloudflare.

## Getting Started

Docker run command :

```
docker run --name dns-updater \
--restart unless-stopped \
-e DNS_UPDATER_EMAIL='<cloudflare email used for registration>' \
-e DNS_UPDATER_KEY='<cloudflare api key>' \
-e DNS_UPDATER_ZONE='<cloudflare dns zoneId to update>' \
dungtri/docker-cloudflare-dns-updater
```

Docker compose :

```
version: "3"

services:
  pihole:
    container_name: dns-updater
    image: dungtri/docker-cloudflare-dns-updater
    environment:
      - DNS_UPDATER_EMAIL='<cloudflare email used for registration>',
      - DNS_UPDATER_KEY='<cloudflare api key>',
      - DNS_UPDATER_ZONE='<cloudflare dns zoneId to update>'
    restart: unless-stopped
```

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
