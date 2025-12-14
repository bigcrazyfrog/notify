return [
  buildAndPush: { env ->
    sh """
      set -euo pipefail

      DEST1="${env.IMAGE_NAME}:${env.IMAGE_TAG}"

      ARGS="--context \$(pwd)/${env.CONTEXT_DIR} \\
            --dockerfile \$(pwd)/${env.DOCKERFILE} \\
            --destination \${DEST1}"

      if [ "${env.PUSH_LATEST}" = "true" ]; then
        ARGS="\$ARGS --destination ${env.IMAGE_NAME}:latest"
      fi

      echo "Running kaniko with args: \$ARGS"
      /kaniko/executor \$ARGS
    """
  }
]
