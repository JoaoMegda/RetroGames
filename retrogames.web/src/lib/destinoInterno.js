export function obterDestinoInterno(voltarPara, origem) {
  if (!voltarPara?.startsWith('/')) return '/';

  try {
    const destino = new URL(voltarPara, origem);
    const caminho = decodeURIComponent(destino.pathname);
    if (destino.origin !== origem || /^\/[\\/]/.test(caminho) || /^\/(login|cadastro)\/?$/i.test(caminho)) {
      return '/';
    }
    return destino.pathname + destino.search + destino.hash;
  } catch {
    return '/';
  }
}
