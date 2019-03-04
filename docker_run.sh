docker run --name dns-updater \
--restart unless-stopped \
-e DNS_UPDATER_EMAIL='<cloudflare email used for registration>' \
-e DNS_UPDATER_KEY='<cloudflare api key>' \
-e DNS_UPDATER_ZONE='<cloudflare dns zoneId to update>' \
dungtri/docker-cloudflare-dns-updater