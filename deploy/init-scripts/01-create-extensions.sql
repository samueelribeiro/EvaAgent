-- Habilita extensões necessárias

-- UUID
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- pgvector para embeddings (RAG)
-- Descomente a linha abaixo se tiver a extensão instalada
-- CREATE EXTENSION IF NOT EXISTS vector;

-- pg_trgm para busca de texto
CREATE EXTENSION IF NOT EXISTS pg_trgm;

-- hstore para armazenamento chave-valor
CREATE EXTENSION IF NOT EXISTS hstore;
