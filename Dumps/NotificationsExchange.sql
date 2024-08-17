--
-- PostgreSQL database dump
--

-- Dumped from database version 16.3
-- Dumped by pg_dump version 16.3

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


--
-- Name: create_notifications_trigger(); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.create_notifications_trigger()
    LANGUAGE plpgsql
    AS $$
BEGIN
    LOOP
        PERFORM pg_sleep(10);
        CALL generate_and_insert_notifications();
    END LOOP;
END;
$$;


ALTER PROCEDURE public.create_notifications_trigger() OWNER TO postgres;

--
-- Name: generate_and_insert_notifications(); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.generate_and_insert_notifications()
    LANGUAGE plpgsql
    AS $$
  BEGIN
    INSERT INTO notifications (content, value, quality) VALUES (CASE ROUND(RANDOM() * 10)
            WHEN 0 THEN 'Сработала аварийная система защиты турбины.'
            WHEN 1 THEN 'Предупреждение: уровень масла в системе управления турбиной ниже нормы.'
      WHEN 2 THEN 'Система обнаружила потенциальную утечку в системе охлаждения турбины.'
      WHEN 3 THEN 'Регулирующие клапаны успешно прошли тестирование на расхаживание.'
      WHEN 4 THEN 'Система управления обнаружила задержку в реакции клапанов безопасности.'
      WHEN 5 THEN 'Система маслоснабжения включена: состояние - норма'
      WHEN 6 THEN 'Боек безопасности №1 сработал.'
      WHEN 7 THEN 'Боек безопасности №2 сработал.'
      WHEN 8 THEN 'Нечувствительность РК: превышено значение на открытие.'
      WHEN 9 THEN 'Расхаживание бойков - выполнено.'
            ELSE 'Заряд ГА - норма.'
        END, RANDOM() * 100, CASE ROUND(RANDOM() * 2) 
              WHEN 0 THEN 'Y'
              ELSE 'N'
   END);
COMMIT;
END;
$$;


ALTER PROCEDURE public.generate_and_insert_notifications() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: notifications; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.notifications (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    content character varying(255),
    value real,
    quality character(1),
    creationdate timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.notifications OWNER TO postgres;

--
-- Data for Name: notifications; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.notifications (id, content, value, quality, creationdate) FROM stdin;
37da7283-8937-4e43-9cb0-47d81dfd2c0e	Боек безопасности №2 сработал.	77.55263	N	2024-08-11 15:01:31.953559
1b74bc69-87b0-44a9-8063-aa59197ad2e6	Боек безопасности №1 сработал.	14.815133	N	2024-08-11 15:01:41.979974
\.


--
-- Name: notifications notifications_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.notifications
    ADD CONSTRAINT notifications_pkey PRIMARY KEY (id);


--
-- PostgreSQL database dump complete
--

