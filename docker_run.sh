docker run --name dns-updater \
--restart unless-stopped \
-e DNS_UPDATER_EMAIL='<cloudflare email used for registration>' \
-e DNS_UPDATER_KEY='<cloudflare api key>' \
-e DNS_UPDATER_ZONE='<cloudflare dns zoneId to update>' \
# (Optional) Delay between checks current ip & update when necessary (default: 30000).
# -e SCHEDULER_CHECK_DELAY=<custom delay>
dungtri/docker-cloudflare-dns-updater