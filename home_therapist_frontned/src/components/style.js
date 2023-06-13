import styled from "styled-components";

export const LayoutMarTop=styled.div`

padding-top:70px;

`

export const Image=styled.div`
width: 100%;
height:450px;
  background-image: ${(props) => `url(${props.url})`};
  background-position: center;
  background-size: cover;

`