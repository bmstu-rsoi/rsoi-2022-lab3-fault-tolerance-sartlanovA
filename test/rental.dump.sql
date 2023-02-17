--
-- PostgreSQL database dump
--

-- Dumped from database version 14.2
-- Dumped by pg_dump version 14.2

-- Started on 2022-12-16 00:15:08

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

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 210 (class 1259 OID 49241)
-- Name: rental; Type: TABLE; Schema: public; Owner: program
--

CREATE TABLE public.rental (
    id integer NOT NULL,
    rental_uid uuid NOT NULL,
    username character varying(80) NOT NULL,
    payment_uid uuid NOT NULL,
    car_uid uuid NOT NULL,
    date_from timestamp with time zone NOT NULL,
    date_to timestamp with time zone NOT NULL,
    status character varying(20) NOT NULL,
    CONSTRAINT rental_status_check CHECK (((status)::text = ANY ((ARRAY['IN_PROGRESS'::character varying, 'FINISHED'::character varying, 'CANCELED'::character varying])::text[])))
);


ALTER TABLE public.rental OWNER TO program;

--
-- TOC entry 209 (class 1259 OID 49240)
-- Name: rental_id_seq; Type: SEQUENCE; Schema: public; Owner: program
--

CREATE SEQUENCE public.rental_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.rental_id_seq OWNER TO program;

--
-- TOC entry 3316 (class 0 OID 0)
-- Dependencies: 209
-- Name: rental_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: program
--

ALTER SEQUENCE public.rental_id_seq OWNED BY public.rental.id;


--
-- TOC entry 3164 (class 2604 OID 49244)
-- Name: rental id; Type: DEFAULT; Schema: public; Owner: program
--

ALTER TABLE ONLY public.rental ALTER COLUMN id SET DEFAULT nextval('public.rental_id_seq'::regclass);


--
-- TOC entry 3317 (class 0 OID 0)
-- Dependencies: 209
-- Name: rental_id_seq; Type: SEQUENCE SET; Schema: public; Owner: program
--

SELECT pg_catalog.setval('public.rental_id_seq', 1, false);


--
-- TOC entry 3167 (class 2606 OID 49247)
-- Name: rental rental_pkey; Type: CONSTRAINT; Schema: public; Owner: program
--

ALTER TABLE ONLY public.rental
    ADD CONSTRAINT rental_pkey PRIMARY KEY (id);


--
-- TOC entry 3169 (class 2606 OID 49249)
-- Name: rental rental_rental_uid_key; Type: CONSTRAINT; Schema: public; Owner: program
--

ALTER TABLE ONLY public.rental
    ADD CONSTRAINT rental_rental_uid_key UNIQUE (rental_uid);


-- Completed on 2022-12-16 00:15:09

--
-- PostgreSQL database dump complete
--

