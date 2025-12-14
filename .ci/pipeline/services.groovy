return [
  // Централизованный маппинг: SERVICE -> (context, dockerfile, image)
  resolveService: { String service, env ->
    if (!env.REGISTRY?.trim()) {
      error("env.REGISTRY is not set. Define REGISTRY in Jenkins job/global env/credentials, e.g. registry.example.com/namespace")
    }

    def cfg = [
      gateway: [
        context:   'src/NotifySystem',
        dockerfile:'src/NotifySystem/NotifySystem.Gateway/Dockerfile',
        image:     "${env.REGISTRY}/notify-gateway"
      ],
      email: [
        context:   'src/NotifySystem',
        dockerfile:'src/NotifySystem/Services/EmailService/EmailService/Dockerfile',
        image:     "${env.REGISTRY}/notify-email"
      ],
      push: [
        context:   'src/NotifySystem',
        dockerfile:'src/NotifySystem/Services/PushService/PushService/Dockerfile',
        image:     "${env.REGISTRY}/notify-push"
      ],
      telegram: [
        // TODO: поправь пути под свой репозиторий
        context:   'src/NotifySystem',
        dockerfile:'src/NotifySystem/Services/TelegramService/TelegramService/Dockerfile',
        image:     "${env.REGISTRY}/notify-telegram"
      ],
    ]

    if (!cfg.containsKey(service)) {
      error("Unknown SERVICE=${service}. Allowed: ${cfg.keySet().join(', ')}")
    }

    return cfg[service]
  }
]
