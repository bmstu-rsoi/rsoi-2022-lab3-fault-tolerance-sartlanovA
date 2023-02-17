--
-- PostgreSQL database dump
--

-- Dumped from database version 14.2
-- Dumped by pg_dump version 14.2

-- Started on 2022-12-16 00:14:19

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
-- TOC entry 210 (class 1259 OID 49252)
-- Name: payment; Type: TABLE; Schema: public; Owner: program
--

CREATE TABLE public.payment (
    id integer NOT NULL,
    payment_uid uuid NOT NULL,
    status character varying(20) NOT NULL,
    price integer NOT NULL,
    CONSTRAINT payment_status_check CHECK (((status)::text = ANY ((ARRAY['PAID'::character varying, 'CANCELED'::character varying])::text[])))
);


ALTER TABLE public.payment OWNER TO program;

--
-- TOC entry 209 (class 1259 OID 49251)
-- Name: payment_id_seq; Type: SEQUENCE; Schema: public; Owner: program
--

CREATE SEQUENCE public.payment_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.payment_id_seq OWNER TO program;

--
-- TOC entry 3314 (class 0 OID 0)
-- Dependencies: 209
-- Name: payment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: program
--

ALTER SEQUENCE public.payment_id_seq OWNED BY public.payment.id;


--
-- TOC entry 3164 (class 2604 OID 49255)
-- Name: payment id; Type: DEFAULT; Schema: public; Owner: program
--

ALTER TABLE ONLY public.payment ALTER COLUMN id SET DEFAULT nextval('public.payment_id_seq'::regclass);


--
-- TOC entry 3315 (class 0 OID 0)
-- Dependencies: 209
-- Name: payment_id_seq; Type: SEQUENCE SET; Schema: public; Owner: program
--

SELECT pg_catalog.setval('public.payment_id_seq', 1, false);


--
-- TOC entry 3167 (class 2606 OID 49258)
-- Name: payment payment_pkey; Type: CONSTRAINT; Schema: public; Owner: program
--

ALTER TABLE ONLY public.payment
    ADD CONSTRAINT payment_pkey PRIMARY KEY (id);


-- Completed on 2022-12-16 00:14:19

--
-- PostgreSQL database dump complete
--

