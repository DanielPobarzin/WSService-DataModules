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
-- Name: update_client(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.update_client() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
     UPDATE db_connection SET client_id = NEW.client_id WHERE client_id = OLD.client_id;
    UPDATE connect_settings SET client_id = NEW.client_id WHERE client_id = OLD.client_id;
	UPDATE kafka_settings SET client_id = NEW.client_id WHERE client_id = OLD.client_id;
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.update_client() OWNER TO postgres;

--
-- Name: update_connect(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.update_connect() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE db_connection SET client_id = NEW.client_id WHERE client_id = OLD.client_id;
    UPDATE client_settings SET client_id = NEW.client_id WHERE client_id = OLD.client_id;
	UPDATE kafka_settings SET client_id = NEW.client_id WHERE client_id = OLD.client_id;
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.update_connect() OWNER TO postgres;

--
-- Name: update_db(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.update_db() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE client_settings SET client_id = NEW.client_id WHERE client_id = OLD.client_id;
    UPDATE connect_settings SET client_id = NEW.client_id WHERE client_id = OLD.client_id;
	UPDATE kafka_settings SET client_id = NEW.client_id WHERE client_id = OLD.client_id;
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.update_db() OWNER TO postgres;

--
-- Name: update_kafka(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.update_kafka() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
   	UPDATE client_settings SET client_id = NEW.client_id WHERE client_id = OLD.client_id;
    UPDATE connect_settings SET client_id = NEW.client_id WHERE client_id = OLD.client_id;
	UPDATE db_connection SET client_id = NEW.client_id WHERE client_id = OLD.client_id;
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.update_kafka() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: client_configuration; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.client_configuration (
    client_id uuid NOT NULL,
    database_type character varying(20),
    alarm_connection_string text,
    notify_connection_string text,
    caching boolean,
    mode character varying(20),
    address_to_alarm text,
    address_to_notify text,
    consumer_bootstrap_server character varying(255),
    producer_bootstrap_server character varying(255),
    system_id uuid
);


ALTER TABLE public.client_configuration OWNER TO postgres;

--
-- Name: client_settings; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.client_settings (
    client_id uuid NOT NULL,
    caching boolean,
    mode character varying(20)
);


ALTER TABLE public.client_settings OWNER TO postgres;

--
-- Name: connect_settings; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.connect_settings (
    client_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    address_to_alarm text,
    address_to_notify text
);


ALTER TABLE public.connect_settings OWNER TO postgres;

--
-- Name: db_connection; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.db_connection (
    client_id uuid NOT NULL,
    database_type character varying(20),
    alarm_connection_string text,
    notify_connection_string text
);


ALTER TABLE public.db_connection OWNER TO postgres;

--
-- Name: kafka_settings; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.kafka_settings (
    client_id uuid NOT NULL,
    consumer_bootstrap_server character varying(255),
    producer_bootstrap_server character varying(255)
);


ALTER TABLE public.kafka_settings OWNER TO postgres;

--
-- Data for Name: client_configuration; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.client_configuration (client_id, database_type, alarm_connection_string, notify_connection_string, caching, mode, address_to_alarm, address_to_notify, consumer_bootstrap_server, producer_bootstrap_server, system_id) FROM stdin;
11341fac-11e6-8b0c-aa4c-cfcf5346a113	PostgreSQL	host=localhost;port=5432;Database=AlarmsExchangeClients;Username=postgres;Password=19346jaidj	host=localhost;port=5432;Database=NotificationsExchangeClients;Username=postgres;Password=19346jaidj	f	Manual	https://localhost:8080/hubs/AlarmHub/Send	https://localhost:8080/hubs/NotificationHub/Send	localhost:9092	localhost:9092	11341fac-11e6-8b0c-aa4c-cfcf5346a113
\.


--
-- Data for Name: client_settings; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.client_settings (client_id, caching, mode) FROM stdin;
11341fac-11e6-8b0c-aa4c-cfcf5346a113	t	Automatic
\.


--
-- Data for Name: connect_settings; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.connect_settings (client_id, address_to_alarm, address_to_notify) FROM stdin;
11341fac-11e6-8b0c-aa4c-cfcf5346a113	https://localhost:8080/hubs/AlarmHub/Send	https://localhost:8080/hubs/NotificationHub/Send
\.


--
-- Data for Name: db_connection; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.db_connection (client_id, database_type, alarm_connection_string, notify_connection_string) FROM stdin;
11341fac-11e6-8b0c-aa4c-cfcf5346a113	PostgreSQL	host=localhost;port=5432;Database=AlarmsExchangeClients;Username=postgres;Password=19346jaidj	host=localhost;port=5432;Database=NotificationsExchangeClients;Username=postgres;Password=19346jaidj
\.


--
-- Data for Name: kafka_settings; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.kafka_settings (client_id, consumer_bootstrap_server, producer_bootstrap_server) FROM stdin;
11341fac-11e6-8b0c-aa4c-cfcf5346a113	localhost:9092	localhost:9092
\.


--
-- Name: client_configuration client_configuration_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.client_configuration
    ADD CONSTRAINT client_configuration_pkey PRIMARY KEY (client_id);


--
-- Name: client_settings client_settings_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.client_settings
    ADD CONSTRAINT client_settings_pkey PRIMARY KEY (client_id);


--
-- Name: connect_settings connect_settings_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.connect_settings
    ADD CONSTRAINT connect_settings_pkey PRIMARY KEY (client_id);


--
-- Name: db_connection db_connection_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.db_connection
    ADD CONSTRAINT db_connection_pkey PRIMARY KEY (client_id);


--
-- Name: kafka_settings kafka_settings_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.kafka_settings
    ADD CONSTRAINT kafka_settings_pkey PRIMARY KEY (client_id);


--
-- Name: client_settings update_trigger_client; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER update_trigger_client AFTER UPDATE OF client_id ON public.client_settings FOR EACH ROW EXECUTE FUNCTION public.update_client();


--
-- Name: connect_settings update_trigger_connect; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER update_trigger_connect AFTER UPDATE OF client_id ON public.connect_settings FOR EACH ROW EXECUTE FUNCTION public.update_connect();


--
-- Name: db_connection update_trigger_db; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER update_trigger_db AFTER UPDATE OF client_id ON public.db_connection FOR EACH ROW EXECUTE FUNCTION public.update_db();


--
-- Name: kafka_settings update_trigger_kafka; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER update_trigger_kafka AFTER UPDATE OF client_id ON public.kafka_settings FOR EACH ROW EXECUTE FUNCTION public.update_kafka();


--
-- Name: client_settings client_settings_client_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.client_settings
    ADD CONSTRAINT client_settings_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.connect_settings(client_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: db_connection db_connection_client_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.db_connection
    ADD CONSTRAINT db_connection_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.connect_settings(client_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: kafka_settings kafka_settings_client_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.kafka_settings
    ADD CONSTRAINT kafka_settings_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.connect_settings(client_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

