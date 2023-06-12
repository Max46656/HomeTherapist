import styled from "styled-components";

export const LayoutMarTop=styled.div`

padding-top:80px;

`

export const Image=styled.div`
width: 100%;
height:${(props) => props.height}px;
  background-image: ${(props) => `url(${props.url})`};
  background-position: center;
  background-size: cover;

    @media screen and (max-width: 420px){
      height:300px;
    }
  
  
  }

`


