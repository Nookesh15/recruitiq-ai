"""
RIQAI-29: Optional LLM client — OpenAI or Anthropic.

Usage:
    result = await llm_complete(prompt)
    if result is None:
        # LLM not configured or call failed — use regex fallback

Set environment variables:
    LLM_PROVIDER = openai | anthropic | none
    LLM_API_KEY  = sk-... or sk-ant-...
    LLM_MODEL    = gpt-4o-mini | claude-haiku-4-5-20251001 | etc.
"""
from __future__ import annotations

import asyncio
import logging

import httpx

from app.core.config import settings

logger = logging.getLogger(__name__)

# Default models when LLM_MODEL is not set
_DEFAULT_MODELS: dict[str, str] = {
    "openai":    "gpt-4o-mini",
    "anthropic": "claude-haiku-4-5-20251001",
}


def _is_enabled() -> bool:
    return (
        settings.llm_provider in ("openai", "anthropic")
        and bool(settings.llm_api_key)
    )


async def llm_complete(prompt: str) -> str | None:
    """
    Send a prompt to the configured LLM. Returns the text response or None
    if LLM is disabled / all retries exhausted.
    """
    if not _is_enabled():
        return None

    model = settings.llm_model or _DEFAULT_MODELS.get(settings.llm_provider, "")
    last_error: Exception | None = None

    for attempt in range(settings.llm_max_retries + 1):
        try:
            if settings.llm_provider == "openai":
                return await _call_openai(prompt, model)
            else:
                return await _call_anthropic(prompt, model)
        except (httpx.TimeoutException, httpx.HTTPStatusError) as exc:
            last_error = exc
            if attempt < settings.llm_max_retries:
                await asyncio.sleep(1.5 ** attempt)
        except Exception as exc:
            logger.warning("LLM call failed (non-retryable): %s", exc)
            return None

    logger.warning("LLM call failed after %d retries: %s", settings.llm_max_retries, last_error)
    return None


async def _call_openai(prompt: str, model: str) -> str:
    async with httpx.AsyncClient(timeout=settings.llm_timeout) as client:
        resp = await client.post(
            "https://api.openai.com/v1/chat/completions",
            headers={"Authorization": f"Bearer {settings.llm_api_key}"},
            json={
                "model": model,
                "messages": [{"role": "user", "content": prompt}],
                "temperature": 0.2,
                "max_tokens": 1024,
            },
        )
        resp.raise_for_status()
        return resp.json()["choices"][0]["message"]["content"]


async def _call_anthropic(prompt: str, model: str) -> str:
    async with httpx.AsyncClient(timeout=settings.llm_timeout) as client:
        resp = await client.post(
            "https://api.anthropic.com/v1/messages",
            headers={
                "x-api-key": settings.llm_api_key,
                "anthropic-version": "2023-06-01",
                "content-type": "application/json",
            },
            json={
                "model": model,
                "max_tokens": 1024,
                "messages": [{"role": "user", "content": prompt}],
            },
        )
        resp.raise_for_status()
        return resp.json()["content"][0]["text"]
