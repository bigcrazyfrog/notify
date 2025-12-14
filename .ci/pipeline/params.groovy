return [
  // Единый расчёт IMAGE_TAG + корректная прокидка boolean как строки для sh
  resolveParams: { params, env ->
    def tag = resolveTag(params, env)
    def pushLatestStr = (params.PUSH_LATEST == true) ? 'true' : 'false'

    return [
      imageTag: tag,
      pushLatestStr: pushLatestStr
    ]
  },

  resolveTag: { params, env ->
    def t = (params.TAG != null) ? params.TAG.trim() : ''
    return t ? t : "${env.BUILD_NUMBER}"
  }
]
