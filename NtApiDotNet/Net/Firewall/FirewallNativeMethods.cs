﻿//  Copyright 2021 Google LLC. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using NtApiDotNet.Utilities.Reflection;
using NtApiDotNet.Win32;
using NtApiDotNet.Win32.Rpc.Transport;
using NtApiDotNet.Win32.Security.Native;
using System;
using System.Runtime.InteropServices;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace NtApiDotNet.Net.Firewall
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPM_DISPLAY_DATA0
    {
        /* [unique][string] */
        [MarshalAs(UnmanagedType.LPWStr)]
        public string name;
        /* [unique][string] */
        [MarshalAs(UnmanagedType.LPWStr)]
        public string description;
    }

    [StructLayout(LayoutKind.Sequential)]
    class FWPM_SESSION0
    {
        public Guid sessionKey;
        public FWPM_DISPLAY_DATA0 displayData;
        public FirewallSessionFlags flags;
        public int txnWaitTimeoutInMSec;
        public int processId;
        public IntPtr sid; // SID* 
        [MarshalAs(UnmanagedType.LPWStr)]
        public string username;
        [MarshalAs(UnmanagedType.Bool)]
        public bool kernelMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FWP_BYTE_BLOB
    {
        public int size;
        /* [unique][size_is] */
        public IntPtr data;

        public byte[] ToArray()
        {
            if (size <= 0 || data == IntPtr.Zero)
            {
                return new byte[0];
            }
            byte[] ret = new byte[size];
            Marshal.Copy(data, ret, 0, ret.Length);
            return ret;
        }

        public Guid ToGuid()
        {
            var bytes = ToArray();
            if (bytes.Length != 16)
                return Guid.Empty;
            return new Guid(bytes);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FWP_RANGE0
    {
        public FWP_VALUE0 valueLow;
        public FWP_VALUE0 valueHigh;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct FWP_VALUE0_UNION
    {
        [FieldOffset(0)]
        public byte uint8;
        [FieldOffset(0)]
        public ushort uint16;
        [FieldOffset(0)]
        public uint uint32;
        [FieldOffset(0)]
        public IntPtr uint64; // UINT64*
        [FieldOffset(0)]
        public sbyte int8;
        [FieldOffset(0)]
        public short int16;
        [FieldOffset(0)]
        public int int32;
        [FieldOffset(0)]
        public IntPtr int64; // INT64* 
        [FieldOffset(0)]
        public float float32;
        [FieldOffset(0)]
        public IntPtr double64; // double* 
        [FieldOffset(0)]
        public IntPtr byteArray16; // FWP_BYTE_ARRAY16* 
        [FieldOffset(0)]
        public IntPtr byteBlob; // FWP_BYTE_BLOB*
        [FieldOffset(0)]
        public IntPtr sid; // SID* 
        [FieldOffset(0)]
        public IntPtr sd; // FWP_BYTE_BLOB* 
        [FieldOffset(0)]
        public IntPtr tokenInformation; // FWP_TOKEN_INFORMATION* 
        [FieldOffset(0)]
        public IntPtr tokenAccessInformation; // FWP_BYTE_BLOB* 
        [FieldOffset(0)]
        public IntPtr unicodeString; // LPWSTR 
        [FieldOffset(0)]
        public IntPtr byteArray6; // FWP_BYTE_ARRAY6* 
        [FieldOffset(0)]
        public IntPtr bitmapArray64; // FWP_BITMAP_ARRAY64*
        [FieldOffset(0)]
        public IntPtr v4AddrMask; // FWP_V4_ADDR_AND_MASK* 
        [FieldOffset(0)]
        public IntPtr v6AddrMask; // FWP_V6_ADDR_AND_MASK* 
        [FieldOffset(0)]
        public IntPtr rangeValue; // FWP_RANGE0* 
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FWP_VALUE0
    {
        public FirewallDataType type;
        public FWP_VALUE0_UNION value;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct FWPM_ACTION0_UNION
    {
        [FieldOffset(0)]
        public Guid filterType;
        [FieldOffset(0)]
        public Guid calloutKey;
        [FieldOffset(0)]
        public byte bitmapIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FWPM_ACTION0
    {
        public FirewallActionType type;
        public FWPM_ACTION0_UNION action;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct FWPM_FILTER0_UNION
    {
        [FieldOffset(0)]
        public ulong rawContext;
        [FieldOffset(0)]
        public Guid providerContextKey;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPM_FILTER0
    {
        public Guid filterKey;
        public FWPM_DISPLAY_DATA0 displayData;
        public FirewallFilterFlags flags;
        public IntPtr providerKey; // GUID*
        public FWP_BYTE_BLOB providerData;
        public Guid layerKey;
        public Guid subLayerKey;
        public FWP_VALUE0 weight;
        public int numFilterConditions;
        public IntPtr filterCondition; // FWPM_FILTER_CONDITION0* 
        public FWPM_ACTION0 action;
        public FWPM_FILTER0_UNION context;
        public IntPtr reserved; // GUID* 
        public ulong filterId;
        public FWP_VALUE0 effectiveWeight;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPM_FILTER_CONDITION0
    {
        public Guid fieldKey;
        public FirewallMatchType matchType;
        public FWP_VALUE0 conditionValue;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWP_V4_ADDR_AND_MASK
    {
        public uint addr;
        public uint mask;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWP_V6_ADDR_AND_MASK
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] addr;
        public byte prefixLength;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWP_TOKEN_INFORMATION
    {
        public int sidCount;
        public IntPtr sids; // PSID_AND_ATTRIBUTES 
        public int restrictedSidCount;
        public IntPtr restrictedSids; // PSID_AND_ATTRIBUTES 
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWP_BITMAP_ARRAY64
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] bitmapArray64;
    }

    public enum FWP_FILTER_ENUM_TYPE
    {
        FWP_FILTER_ENUM_FULLY_CONTAINED = 0,
        FWP_FILTER_ENUM_OVERLAPPING = FWP_FILTER_ENUM_FULLY_CONTAINED + 1,
        FWP_FILTER_ENUM_TYPE_MAX = FWP_FILTER_ENUM_OVERLAPPING + 1
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    class FWPM_FILTER_ENUM_TEMPLATE0
    {
        public IntPtr providerKey;
        public Guid layerKey;
        public FWP_FILTER_ENUM_TYPE enumType;
        public FirewallFilterEnumFlags flags;
        public IntPtr providerContextTemplate; // FWPM_PROVIDER_CONTEXT_ENUM_TEMPLATE0*
        public int numFilterConditions;
        public IntPtr filterCondition; // FWPM_FILTER_CONDITION0* 
        public FirewallActionType actionMask;
        public IntPtr calloutKey; // GUID*
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPS_INCOMING_VALUES0
    {
        public ushort layerId;
        public int valueCount;
        public IntPtr incomingValue; // FWPS_INCOMING_VALUE0* 
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPS_CLASSIFY_OUT0
    {
        public FirewallActionType actionType;
        public ulong outContext;
        public ulong filterId;
        public FirewallRightActions rights;
        public FirewallClassifyOutFlags flags;
        public int reserved;
    }

    public enum FirewallIpVersion
    {
        [SDKName("FWP_IP_VERSION_V4")]
        V4 = 0,
        [SDKName("FWP_IP_VERSION_V6")]
        V6,
        [SDKName("FWP_IP_VERSION_NONE")]
        None
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FWPM_FIELD0
    {
        /* [ref] */
        public IntPtr fieldKey; // GUID* 
        public FirewallFieldType type;
        public FirewallDataType dataType;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FWPM_LAYER0
    {
        public Guid layerKey;
        public FWPM_DISPLAY_DATA0 displayData;
        public FirewallLayerFlags flags;
        public int numFields;
        public IntPtr field; // FWPM_FIELD0*
        public Guid defaultSubLayerKey;
        public ushort layerId;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FWPM_SUBLAYER0
    {
        public Guid subLayerKey;
        public FWPM_DISPLAY_DATA0 displayData;
        public FirewallSubLayerFlags flags;
        /* [unique] */
        public IntPtr providerKey; // GUID* 
        public FWP_BYTE_BLOB providerData;
        public ushort weight;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FWPM_CALLOUT0
    {
        public Guid calloutKey;
        public FWPM_DISPLAY_DATA0 displayData;
        public FirewallCalloutFlags flags;
        public IntPtr providerKey; // GUID* 
        public FWP_BYTE_BLOB providerData;
        public Guid applicableLayer;
        public int calloutId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPM_PROVIDER0
    {
        public Guid providerKey;
        public FWPM_DISPLAY_DATA0 displayData;
        public FirewallProviderFlags flags;
        public FWP_BYTE_BLOB providerData;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string serviceName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_KEY_MANAGER0
    {
        public Guid keyManagerKey;
        public FWPM_DISPLAY_DATA0 displayData;
        public IPsecKeyManagerFlags flags;
        public byte keyDictationTimeoutHint;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPS_ALE_ENDPOINT_PROPERTIES0
    {
        public ulong endpointId;
        public FirewallIpVersion ipVersion; // FWP_IP_VERSION 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] localAddress;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] remoteAddress;
        public byte ipProtocol;
        public ushort localPort;
        public ushort remotePort;
        public long localTokenModifiedId;
        public ulong mmSaId;
        public ulong qmSaId;
        public uint ipsecStatus;
        public uint flags;
        public FWP_BYTE_BLOB appId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPS_ALE_ENDPOINT_ENUM_TEMPLATE0
    {
        FWP_VALUE0 localSubNet;
        FWP_VALUE0 remoteSubNet;
        FWP_VALUE0 ipProtocol;
        FWP_VALUE0 localPort;
        FWP_VALUE0 remotePort;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IKEEXT_TRAFFIC0
    {
        public FirewallIpVersion ipVersion; // FWP_IP_VERSION 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] localAddress;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] remoteAddress;
        public ulong authIpFilterId;
    }

    [SDKName("IKEEXT_KEY_MODULE_TYPE")]
    public enum IkeExtKeyModuleType
    {
        [SDKName("IKEEXT_KEY_MODULE_IKE")]
        Ike = 0,
        [SDKName("IKEEXT_KEY_MODULE_AUTHIP")]
        AuthIP = (Ike + 1),
        [SDKName("IKEEXT_KEY_MODULE_IKEV2")]
        IkeV2 = (AuthIP + 1),
    }

    [SDKName("IKEEXT_DH_GROUP")]
    public enum IkeextDHGroup
    {
        [SDKName("IKEEXT_DH_GROUP_NONE")]
        None = 0,
        [SDKName("IKEEXT_DH_GROUP_1")]
        Group1 = (None + 1),
        [SDKName("IKEEXT_DH_GROUP_2")]
        Group2 = (Group1 + 1),
        [SDKName("IKEEXT_DH_GROUP_14")]
        Group14 = (Group2 + 1),
        [SDKName("IKEEXT_DH_GROUP_2048")]
        Group2048 = Group14,
        [SDKName("IKEEXT_DH_ECP_256")]
        ECP256 = (Group2048 + 1),
        [SDKName("IKEEXT_DH_ECP_384")]
        ECP384 = (ECP256 + 1),
        [SDKName("IKEEXT_DH_GROUP_24")]
        Group24 = (ECP384 + 1),
    }

    [SDKName("IKEEXT_INTEGRITY_TYPE")]
    public enum IkeextIntegrityType
    {
        [SDKName("IKEEXT_INTEGRITY_MD5")]
        MD5 = 0,
        [SDKName("IKEEXT_INTEGRITY_SHA1")]
        SHA1 = (MD5 + 1),
        [SDKName("IKEEXT_INTEGRITY_SHA_256")]
        SHA256 = (SHA1 + 1),
        [SDKName("IKEEXT_INTEGRITY_SHA_384")]
        SHA384 = (SHA256 + 1)
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IKEEXT_INTEGRITY_ALGORITHM0
    {
        public IkeextIntegrityType algoIdentifier;
    }

    [SDKName("IKEEXT_CIPHER_TYPE")]
    public enum IkeExtCipherType
    {
        [SDKName("IKEEXT_CIPHER_DES")]
        DES = 0,
        [SDKName("IKEEXT_CIPHER_3DES")]
        TripleDES = (DES + 1),
        [SDKName("IKEEXT_CIPHER_AES_128")]
        AES128 = (TripleDES + 1),
        [SDKName("IKEEXT_CIPHER_AES_192")]
        AES192 = (AES128 + 1),
        [SDKName("IKEEXT_CIPHER_AES_256")]
        AES256 = (AES192 + 1),
        [SDKName("IKEEXT_CIPHER_AES_GCM_128_16ICV")]
        AESGCM128_16ICV = (AES256 + 1),
        [SDKName("IKEEXT_CIPHER_AES_GCM_256_16ICV")]
        AESGCM256_16ICV = (AESGCM128_16ICV + 1)
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IKEEXT_CIPHER_ALGORITHM0
    {
        public IkeExtCipherType algoIdentifier;
        public int keyLen;
        public int rounds;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IKEEXT_PROPOSAL0
    {
        public IKEEXT_CIPHER_ALGORITHM0 cipherAlgorithm;
        public IKEEXT_INTEGRITY_ALGORITHM0 integrityAlgorithm;
        public uint maxLifetimeSeconds;
        public IkeextDHGroup dhGroup;
        public uint quickModeLimit;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IKEEXT_COOKIE_PAIR0
    {
        public ulong initiator;
        public ulong responder;
    }

    [Flags]
    public enum IkeextCertCredentialFlags
    {
        None = 0,
        [SDKName("IKEEXT_CERT_CREDENTIAL_FLAG_NAP_CERT")]
        NapCert = 0x00000001,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IKEEXT_CERTIFICATE_CREDENTIAL1
    {
        public FWP_BYTE_BLOB subjectName;
        public FWP_BYTE_BLOB certHash;
        public IkeextCertCredentialFlags flags;
        public FWP_BYTE_BLOB certificate;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IKEEXT_NAME_CREDENTIAL0
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string principalName;
    }

    [SDKName("IKEEXT_AUTHENTICATION_METHOD_TYPE")]
    public enum IkeextAuthenticationMethodType
    {
        [SDKName("IKEEXT_PRESHARED_KEY")]
        PreSharedKey = 0,
        [SDKName("IKEEXT_CERTIFICATE")]
        Certificate = (PreSharedKey + 1),
        [SDKName("IKEEXT_KERBEROS")]
        Kerberos = (Certificate + 1),
        [SDKName("IKEEXT_ANONYMOUS")]
        Anonymous = (Kerberos + 1),
        [SDKName("IKEEXT_SSL")]
        Ssl = (Anonymous + 1),
        [SDKName("IKEEXT_NTLM_V2")]
        NtlmV2 = (Ssl + 1),
        [SDKName("IKEEXT_IPV6_CGA")]
        IPv6Cga = (NtlmV2 + 1),
        [SDKName("IKEEXT_CERTIFICATE_ECDSA_P256")]
        CertificateECDSA_P256 = (IPv6Cga + 1),
        [SDKName("IKEEXT_CERTIFICATE_ECDSA_P384")]
        CertificateECDSA_P384 = (CertificateECDSA_P256 + 1),
        [SDKName("IKEEXT_SSL_ECDSA_P256")]
        SslECDSA_P256 = (CertificateECDSA_P384 + 1),
        [SDKName("IKEEXT_SSL_ECDSA_P384")]
        SslECDSA_P384 = (SslECDSA_P256 + 1),
        [SDKName("IKEEXT_EAP")]
        EAP = (SslECDSA_P384 + 1),
        [SDKName("IKEEXT_RESERVED")]
        Reserved = (EAP + 1),
    }

    [SDKName("IKEEXT_AUTHENTICATION_IMPERSONATION_TYPE")]
    public enum IkeextAuthenticationImpersonationType
    {
        [SDKName("IKEEXT_IMPERSONATION_NONE")]
        None = 0,
        [SDKName("IKEEXT_IMPERSONATION_SOCKET_PRINCIPAL")]
        SocketPrincipal = (None + 1),
    }

    [Flags]
    public enum IkeextPreSharedKeyFlags
    {
        None = 0,
        [SDKName("IKEEXT_PSK_FLAG_LOCAL_AUTH_ONLY")]
        LocalAuthOnly = 0x00000001,
        [SDKName("IKEEXT_PSK_FLAG_REMOTE_AUTH_ONLY")]
        RemoteAuthOnly = 0x00000002,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IKEEXT_PRESHARED_KEY_AUTHENTICATION1
    {
        public FWP_BYTE_BLOB presharedKey;
        public IkeextPreSharedKeyFlags flags;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IKEEXT_CREDENTIAL1
    {
        public IkeextAuthenticationMethodType authenticationMethodType;
        public IkeextAuthenticationImpersonationType impersonationType;
        public IntPtr cred;
        // IKEEXT_PRESHARED_KEY_AUTHENTICATION1* presharedKey;
        // IKEEXT_CERTIFICATE_CREDENTIAL1* certificate;
        // IKEEXT_NAME_CREDENTIAL0* name;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IKEEXT_CREDENTIAL_PAIR1
    {
        public IKEEXT_CREDENTIAL1 localCredentials;
        public IKEEXT_CREDENTIAL1 peerCredentials;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IKEEXT_CREDENTIALS1
    {
        public int numCredentials;
        public IntPtr credentials; // IKEEXT_CREDENTIAL_PAIR1*
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_V4_UDP_ENCAPSULATION0
    {
        public ushort localUdpEncapPort;
        public ushort remoteUdpEncapPort;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IKEEXT_SA_DETAILS1
    {
        public ulong saId;
        public IkeExtKeyModuleType keyModuleType;
        public FirewallIpVersion ipVersion;
        public IntPtr v4UdpEncapsulation; // IPSEC_V4_UDP_ENCAPSULATION0*
        public IKEEXT_TRAFFIC0 ikeTraffic;
        public IKEEXT_PROPOSAL0 ikeProposal;
        public IKEEXT_COOKIE_PAIR0 cookiePair;
        public IKEEXT_CREDENTIALS1 ikeCredentials;
        public Guid ikePolicyKey;
        public ulong virtualIfTunnelId;
        public FWP_BYTE_BLOB correlationKey;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPM_NET_EVENT_HEADER2
    {
        public Luid timeStamp;
        public FirewallNetEventFlags flags;
        public FirewallIpVersion ipVersion;
        public byte ipProtocol;
        public uint localAddrV4;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] localAddrV6;
        public uint remoteAddrV4;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] remoteAddrV6;
        public ushort localPort;
        public ushort remotePort;
        public uint scopeId;
        public FWP_BYTE_BLOB appId;
        public IntPtr userId;
        public FirewallAddressFamily addressFamily;
        public IntPtr packageSid;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPM_NET_EVENT2
    {
        public FWPM_NET_EVENT_HEADER2 header;
        public FirewallNetEventType type;
        public IntPtr value;
        /*
        union {
        FWPM_NET_EVENT_IKEEXT_MM_FAILURE1* ikeMmFailure;
        FWPM_NET_EVENT_IKEEXT_QM_FAILURE0* ikeQmFailure;
        FWPM_NET_EVENT_IKEEXT_EM_FAILURE1* ikeEmFailure;
        FWPM_NET_EVENT_CLASSIFY_DROP2* classifyDrop;
        FWPM_NET_EVENT_IPSEC_KERNEL_DROP0* ipsecDrop;
        FWPM_NET_EVENT_IPSEC_DOSP_DROP0* idpDrop;
        FWPM_NET_EVENT_CLASSIFY_ALLOW0* classifyAllow;
        FWPM_NET_EVENT_CAPABILITY_DROP0* capabilityDrop;
        FWPM_NET_EVENT_CAPABILITY_ALLOW0* capabilityAllow;
        FWPM_NET_EVENT_CLASSIFY_DROP_MAC0* classifyDropMac;
        */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPM_NET_EVENT_IPSEC_KERNEL_DROP0
    {
        public NtStatus failureStatus;
        public FirewallDirectionType direction;
        public uint spi;
        public ulong filterId;
        public ushort layerId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPM_NET_EVENT_CLASSIFY_DROP2
    {
        public ulong filterId;
        public ushort layerId;
        public uint reauthReason;
        public uint originalProfile;
        public uint currentProfile;
        public FirewallNetEventDirectionType msFwpDirection;
        [MarshalAs(UnmanagedType.Bool)]
        public bool isLoopback;
        public FWP_BYTE_BLOB vSwitchId;
        public uint vSwitchSourcePort;
        public uint vSwitchDestinationPort;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPM_NET_EVENT_CAPABILITY_DROP0
    {
        public FirewallNetworkCapabilityType networkCapabilityId;
        public ulong filterId;
        [MarshalAs(UnmanagedType.Bool)]
        public bool isLoopback;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPM_NET_EVENT_ENUM_TEMPLATE0
    {
        public Luid startTime;
        public Luid endTime;
        public int numFilterConditions;
        public IntPtr filterCondition;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = false)]
    delegate void FwpmNetEventCallback1(IntPtr context, IntPtr ev);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct FWPM_NET_EVENT_SUBSCRIPTION0
    {
        public IntPtr enumTemplate; // FWPM_NET_EVENT_ENUM_TEMPLATE0
        public int flags;
        public Guid sessionKey;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_SA_CONTEXT1
    {
        public ulong saContextId;
        public IntPtr inboundSa; // IPSEC_SA_DETAILS1* 
        public IntPtr outboundSa; // IPSEC_SA_DETAILS1* 
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_TRAFFIC1
    {
        public FirewallIpVersion ipVersion;
        public uint localAddrV4;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] localAddrV6;
        public uint remoteAddrV4;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] remoteAddrV6;
        public IPsecTrafficType trafficType;
        public ulong trafficTypeId;
        public ushort remotePort;
        public ushort localPort;
        public byte ipProtocol;
        public long localIfLuid;
        public uint realIfProfileId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_SA_LIFETIME0
    {
        public uint lifetimeSeconds;
        public uint lifetimeKilobytes;
        public uint lifetimePackets;
    }

    [SDKName("IPSEC_TRANSFORM_TYPE")]
    public enum IPsecTransformType
    {
        [SDKName("IPSEC_TRANSFORM_AH")]
        AH,
        [SDKName("IPSEC_TRANSFORM_ESP_AUTH")]
        EspAuth,
        [SDKName("IPSEC_TRANSFORM_ESP_CIPHER")]
        EspCipher,
        [SDKName("IPSEC_TRANSFORM_ESP_AUTH_AND_CIPHER")]
        EspAuthAndCipher,
        [SDKName("IPSEC_TRANSFORM_ESP_AUTH_FW")]
        EspAuthFw
    }

    /// <summary>
    /// IPsec authentication type.
    /// </summary>
    [SDKName("IPSEC_AUTH_TYPE")]
    public enum IPsecAuthType
    {
        [SDKName("IPSEC_AUTH_MD5")]
        MD5 = 0,
        [SDKName("IPSEC_AUTH_SHA_1")]
        SHA1 = (MD5 + 1),
        [SDKName("IPSEC_AUTH_SHA_256")]
        SHA256 = (SHA1 + 1),
        [SDKName("IPSEC_AUTH_AES_128")]
        AES128 = (SHA256 + 1),
        [SDKName("IPSEC_AUTH_AES_192")]
        AES192 = (AES128 + 1),
        [SDKName("IPSEC_AUTH_AES_256")]
        AES256 = (AES192 + 1)
    }

    public enum IPsecAuthConfig : byte
    {
        [SDKName("IPSEC_AUTH_CONFIG_HMAC_MD5_96")]
        HMAC_MD5_96 = 0,
        [SDKName("IPSEC_AUTH_CONFIG_HMAC_SHA_1_96")]
        HMAC_SHA1_96 = 1,
        [SDKName("IPSEC_AUTH_CONFIG_HMAC_SHA_256_128")]
        HMAC_SHA256_128 = 2,
        [SDKName("IPSEC_AUTH_CONFIG_GCM_AES_128")]
        GCM_AES128 = 3,
        [SDKName("IPSEC_AUTH_CONFIG_GCM_AES_192")]
        GCM_AES192 = 4,
        [SDKName("IPSEC_AUTH_CONFIG_GCM_AES_256")]
        GCM_AES256 = 5,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_AUTH_TRANSFORM_ID0
    {
        public IPsecAuthType authType;
        public IPsecAuthConfig authConfig;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_AUTH_TRANSFORM0
    {
        public IPSEC_AUTH_TRANSFORM_ID0 authTransformId;
        public IntPtr cryptoModuleId; // IPSEC_CRYPTO_MODULE_ID* -> GUID
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_SA_AUTH_INFORMATION0
    {
        public IPSEC_AUTH_TRANSFORM0 authTransform;
        public FWP_BYTE_BLOB authKey;
    }

    /// <summary>
    /// IPSec Cipher Type.
    /// </summary>
    [SDKName("IPSEC_CIPHER_TYPE")]
    public enum IPsecCipherType
    {
        [SDKName("IPSEC_CIPHER_TYPE_DES")]
        DES = 1,
        [SDKName("IPSEC_CIPHER_TYPE_3DES")]
        TripleDES = (DES + 1),
        [SDKName("IPSEC_CIPHER_TYPE_AES_128")]
        AES128 = (TripleDES + 1),
        [SDKName("IPSEC_CIPHER_TYPE_AES_192")]
        AES192 = (AES128 + 1),
        [SDKName("IPSEC_CIPHER_TYPE_AES_256")]
        AES256 = (AES192 + 1)
    }

    /// <summary>
    /// IPsec Cipher Configuration.
    /// </summary>
    public enum IPsecCipherConfig : byte
    {
        [SDKName("IPSEC_CIPHER_CONFIG_CBC_DES")]
        CBC_DES = 1,
        [SDKName("IPSEC_CIPHER_CONFIG_CBC_3DES")]
        CBC_3DES = 2,
        [SDKName("IPSEC_CIPHER_CONFIG_CBC_AES_128")]
        CBC_AES128 = 3,
        [SDKName("IPSEC_CIPHER_CONFIG_CBC_AES_192")]
        CBC_AES192 = 4,
        [SDKName("IPSEC_CIPHER_CONFIG_CBC_AES_256")]
        CBC_AES256 = 5,
        [SDKName("IPSEC_CIPHER_CONFIG_GCM_AES_128")]
        GCM_AES128 = 6,
        [SDKName("IPSEC_CIPHER_CONFIG_GCM_AES_192")]
        GCM_AES192 = 7,
        [SDKName("IPSEC_CIPHER_CONFIG_GCM_AES_256")]
        GCM_AES256 = 8,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_CIPHER_TRANSFORM_ID0
    {
        public IPsecCipherType cipherType;
        public IPsecCipherConfig cipherConfig;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_CIPHER_TRANSFORM0
    {
        public IPSEC_CIPHER_TRANSFORM_ID0 cipherTransformId;
        public IntPtr cryptoModuleId; // IPSEC_CRYPTO_MODULE_ID* -> GUID
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_SA_CIPHER_INFORMATION0
    {
        public IPSEC_CIPHER_TRANSFORM0 cipherTransform;
        public FWP_BYTE_BLOB cipherKey;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_SA_AUTH_AND_CIPHER_INFORMATION0
    {
        public IPSEC_SA_CIPHER_INFORMATION0 saCipherInformation;
        public IPSEC_SA_AUTH_INFORMATION0 saAuthInformation;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_SA0
    {
        public uint spi;
        public IPsecTransformType saTransformType;
        public IntPtr ptr;
        //IPSEC_SA_AUTH_INFORMATION0            *ahInformation;
        //IPSEC_SA_AUTH_INFORMATION0            *espAuthInformation;
        //IPSEC_SA_CIPHER_INFORMATION0          *espCipherInformation;
        //IPSEC_SA_AUTH_AND_CIPHER_INFORMATION0 *espAuthAndCipherInformation;
        //IPSEC_SA_AUTH_INFORMATION0            *espAuthFwInformation;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_ID0
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string mmTargetName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string emTargetName;
        public int numTokens;
        public IntPtr tokens; // IPSEC_TOKEN0* 
        public ulong explicitCredentials;
        public ulong logonId;
    }

    public enum IPSEC_TOKEN_MODE
    {
        IPSEC_TOKEN_MODE_MAIN = 0, 
        IPSEC_TOKEN_MODE_EXTENDED = (IPSEC_TOKEN_MODE_MAIN + 1) 
    }

    public enum IPSEC_TOKEN_TYPE
    {
        IPSEC_TOKEN_TYPE_MACHINE = 0,
        IPSEC_TOKEN_TYPE_IMPERSONATION = (IPSEC_TOKEN_TYPE_MACHINE + 1)
    }

    public enum IPSEC_TOKEN_PRINCIPAL
    {
        IPSEC_TOKEN_PRINCIPAL_LOCAL = 0,
        IPSEC_TOKEN_PRINCIPAL_PEER = (IPSEC_TOKEN_PRINCIPAL_LOCAL + 1)
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_TOKEN0
    {
        public IPSEC_TOKEN_TYPE type;
        public IPSEC_TOKEN_PRINCIPAL principal;
        public IPSEC_TOKEN_MODE mode;
        public long token;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_SA_BUNDLE1
    {
        public IPsecSecurityAssociationBundleFlags flags;
        public IPSEC_SA_LIFETIME0 lifetime;
        public uint idleTimeoutSeconds;
        public uint ndAllowClearTimeoutSeconds;
        public IntPtr ipsecId; // IPSEC_ID0* 
        public uint napContext;
        public uint qmSaId;
        public int numSAs;
        public IntPtr saList; // IPSEC_SA0* 
        public IntPtr keyModuleState; // IPSEC_KEYMODULE_STATE0* 
        public FirewallIpVersion ipVersion;
        public uint peerV4PrivateAddress;
        public ulong mmSaId;
        public IPsecPerfectForwardSecrecyGroup pfsGroup;
        public Guid saLookupContext;
        public ulong qmFilterId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_KEYMODULE_STATE0
    {
        public Guid keyModuleKey;
        public FWP_BYTE_BLOB stateBlob;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_VIRTUAL_IF_TUNNEL_INFO0
    {
        public ulong virtualIfTunnelId;
        public ulong trafficSelectorId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct IPSEC_SA_DETAILS1
    {
        public FirewallIpVersion ipVersion;
        public FirewallDirectionType saDirection;
        public IPSEC_TRAFFIC1 traffic;
        public IPSEC_SA_BUNDLE1 saBundle;
        public IntPtr udpEncapsulation; // IPSEC_V4_UDP_ENCAPSULATION0 
        public IntPtr transportFilter; // FWPM_FILTER0* 
        public IntPtr virtualIfTunnelInfo; // IPSEC_VIRTUAL_IF_TUNNEL_INFO0* 
    }

    internal static class FirewallNativeMethods
    {
        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmEngineOpen0(
            [Optional] string serverName,
            RpcAuthenticationType authnService,
            SEC_WINNT_AUTH_IDENTITY authIdentity,
            FWPM_SESSION0 session,
            out SafeFwpmEngineHandle engineHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmEngineGetOption0(
            SafeFwpmEngineHandle engineHandle,
            FirewallEngineOption option,
            out SafeFwpmMemoryBuffer value // FWP_VALUE0** 
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmEngineSetOption0(
            SafeFwpmEngineHandle engineHandle,
            FirewallEngineOption option,
            in FWP_VALUE0 value
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmTransactionBegin0(
            SafeFwpmEngineHandle engineHandle,
            FirewallTransactionFlags flags
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmTransactionCommit0(
            SafeFwpmEngineHandle engineHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmTransactionAbort0(
            SafeFwpmEngineHandle engineHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmFilterDestroyEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmFilterCreateEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            SafeBuffer enumTemplate, // FWPM_FILTER_ENUM_TEMPLATE0*
            out IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmFilterEnum0(
           SafeFwpmEngineHandle engineHandle,
           IntPtr enumHandle,
           int numEntriesRequested,
           out SafeFwpmMemoryBuffer entries, // FWPM_FILTER0***
           out int numEntriesReturned
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmFilterGetByKey0(
          SafeFwpmEngineHandle engineHandle,
          in Guid key,
          out SafeFwpmMemoryBuffer filter // FWPM_FILTER0 **
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmFilterGetById0(
          SafeFwpmEngineHandle engineHandle,
          ulong id,
          out SafeFwpmMemoryBuffer filter // FWPM_FILTER0 **
        );


        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmFilterAdd0(
            SafeFwpmEngineHandle engineHandle,
            in FWPM_FILTER0 filter,
            SafeBuffer sd, // PSECURITY_DESCRIPTOR 
            out ulong id
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmFilterDeleteByKey0(
            SafeFwpmEngineHandle engineHandle,
            in Guid key
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmFilterDeleteById0(
            SafeFwpmEngineHandle engineHandle,
            ulong id
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern void FwpmFreeMemory0(
            ref IntPtr p
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmEngineGetSecurityInfo0(
            SafeFwpmEngineHandle engineHandle,
            SecurityInformation securityInfo,
            IntPtr sidOwner,
            IntPtr sidGroup,
            IntPtr dacl,
            IntPtr sacl,
            out SafeFwpmMemoryBuffer securityDescriptor
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmConnectionGetSecurityInfo0(
            SafeFwpmEngineHandle engineHandle,
            SecurityInformation securityInfo,
            IntPtr sidOwner,
            IntPtr sidGroup,
            IntPtr dacl,
            IntPtr sacl,
            out SafeFwpmMemoryBuffer securityDescriptor
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmNetEventsGetSecurityInfo0(
            SafeFwpmEngineHandle engineHandle,
            SecurityInformation securityInfo,
            IntPtr sidOwner,
            IntPtr sidGroup,
            IntPtr dacl,
            IntPtr sacl,
            out SafeFwpmMemoryBuffer securityDescriptor
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmFilterGetSecurityInfoByKey0(
            SafeFwpmEngineHandle engineHandle,
            in Guid key,
            SecurityInformation securityInfo,
            IntPtr sidOwner,
            IntPtr sidGroup,
            IntPtr dacl,
            IntPtr sacl,
            out SafeFwpmMemoryBuffer securityDescriptor
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmCalloutGetSecurityInfoByKey0(
            SafeFwpmEngineHandle engineHandle,
            in Guid key,
            SecurityInformation securityInfo,
            IntPtr sidOwner,
            IntPtr sidGroup,
            IntPtr dacl,
            IntPtr sacl,
            out SafeFwpmMemoryBuffer securityDescriptor
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmLayerGetSecurityInfoByKey0(
            SafeFwpmEngineHandle engineHandle,
            in Guid key,
            SecurityInformation securityInfo,
            IntPtr sidOwner,
            IntPtr sidGroup,
            IntPtr dacl,
            IntPtr sacl,
            out SafeFwpmMemoryBuffer securityDescriptor
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmProviderContextGetSecurityInfoByKey0(
            SafeFwpmEngineHandle engineHandle,
            in Guid key,
            SecurityInformation securityInfo,
            IntPtr sidOwner,
            IntPtr sidGroup,
            IntPtr dacl,
            IntPtr sacl,
            out SafeFwpmMemoryBuffer securityDescriptor
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmProviderCreateEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            SafeBuffer enumTemplate,
            out IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmProviderEnum0(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle,
            int numEntriesRequested,
            out SafeFwpmMemoryBuffer entries, // FWPM_PROVIDER0***
            out int numEntriesReturned
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmProviderGetByKey0(
            SafeFwpmEngineHandle engineHandle,
            in Guid key,
            out SafeFwpmMemoryBuffer provider // FWPM_PROVIDER0 **
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmProviderDestroyEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmProviderGetSecurityInfoByKey0(
            SafeFwpmEngineHandle engineHandle,
            in Guid key,
            SecurityInformation securityInfo,
            IntPtr sidOwner,
            IntPtr sidGroup,
            IntPtr dacl,
            IntPtr sacl,
            out SafeFwpmMemoryBuffer securityDescriptor
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmSubLayerGetSecurityInfoByKey0(
            SafeFwpmEngineHandle engineHandle,
            in Guid key,
            SecurityInformation securityInfo,
            IntPtr sidOwner,
            IntPtr sidGroup,
            IntPtr dacl,
            IntPtr sacl,
            out SafeFwpmMemoryBuffer securityDescriptor
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmLayerCreateEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            SafeBuffer enumTemplate, // FWPM_LAYER_ENUM_TEMPLATE0*
            out IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmLayerEnum0(
           SafeFwpmEngineHandle engineHandle,
           IntPtr enumHandle,
           int numEntriesRequested,
           out SafeFwpmMemoryBuffer entries, // FWPM_LAYER0***
           out int numEntriesReturned
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmLayerGetByKey0(
          SafeFwpmEngineHandle engineHandle,
          in Guid key,
          out SafeFwpmMemoryBuffer layer // FWPM_LAYER0**
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmLayerGetById0(
          SafeFwpmEngineHandle engineHandle,
          ushort id,
          out SafeFwpmMemoryBuffer layer // FWPM_LAYER0** 
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmLayerDestroyEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmSubLayerCreateEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            SafeBuffer enumTemplate, // FWPM_SUBLAYER_ENUM_TEMPLATE0* 
            out IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmSubLayerEnum0(
           SafeFwpmEngineHandle engineHandle,
           IntPtr enumHandle,
           int numEntriesRequested,
           out SafeFwpmMemoryBuffer entries, // FWPM_SUBLAYER0***
           out int numEntriesReturned
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmSubLayerGetByKey0(
            SafeFwpmEngineHandle engineHandle,
            in Guid key,
            out SafeFwpmMemoryBuffer sublayer // FWPM_SUBLAYER0**
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmSubLayerDestroyEnumHandle0(
           SafeFwpmEngineHandle engineHandle,
           IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmSessionCreateEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            SafeBuffer enumTemplate, // FWPM_SESSION_ENUM_TEMPLATE0* 
            out IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmSessionEnum0(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle,
            int numEntriesRequested,
            out SafeFwpmMemoryBuffer entries, // FWPM_SESSION0*** 
            out int numEntriesReturned
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmSessionDestroyEnumHandle0(
           SafeFwpmEngineHandle engineHandle,
           IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmGetAppIdFromFileName0(
            string fileName,
            out SafeFwpmMemoryBuffer appId // FWP_BYTE_BLOB
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmCalloutCreateEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            SafeBuffer enumTemplate,  // const FWPM_CALLOUT_ENUM_TEMPLATE0*
            out IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmCalloutEnum0(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle,
            int numEntriesRequested,
            out SafeFwpmMemoryBuffer entries, // FWPM_CALLOUT0*** 
            out int numEntriesReturned
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmCalloutGetByKey0(
          SafeFwpmEngineHandle engineHandle,
          in Guid key,
          out SafeFwpmMemoryBuffer callout //  FWPM_CALLOUT0 **
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmCalloutDestroyEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error IkeextSaDbGetSecurityInfo0(
            SafeFwpmEngineHandle engineHandle,
            SecurityInformation securityInfo,
            IntPtr sidOwner,
            IntPtr sidGroup,
            IntPtr dacl,
            IntPtr sacl,
            out SafeFwpmMemoryBuffer securityDescriptor
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpsClassifyUser0(
            SafeFwpmEngineHandle engineHandle,
            ushort layerId,
            in FWPS_INCOMING_VALUES0 inFixedValues,
            IntPtr inMetadataValues,
            IntPtr layerData,
            out FWPS_CLASSIFY_OUT0 classifyOut
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error IPsecKeyManagersGet0(
          SafeFwpmEngineHandle engineHandle,
          out SafeFwpmMemoryBuffer entries, // IPSEC_KEY_MANAGER0***
          out int numEntries
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error IPsecKeyManagerGetSecurityInfoByKey0(
            SafeFwpmEngineHandle engineHandle,
            in Guid key,
            SecurityInformation securityInfo,
            IntPtr sidOwner,
            IntPtr sidGroup,
            IntPtr dacl,
            IntPtr sacl,
            out SafeFwpmMemoryBuffer securityDescriptor
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpsOpenToken0(
          SafeFwpmEngineHandle engineHandle,
          Luid modifiedId,
          TokenAccessRights desiredAccess,
          out SafeKernelObjectHandle accessToken
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpsAleEndpointCreateEnumHandle0(
          SafeFwpmEngineHandle engineHandle,
          SafeBuffer enumTemplate, // const FWPS_ALE_ENDPOINT_ENUM_TEMPLATE0* 
          out IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpsAleEndpointDestroyEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpsAleEndpointEnum0(
          SafeFwpmEngineHandle engineHandle,
          IntPtr enumHandle,
          int numEntriesRequested,
          out SafeFwpmMemoryBuffer entries, // FWPS_ALE_ENDPOINT_PROPERTIES0*** 
          out int numEntriesReturned
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpsAleEndpointGetById0(
          SafeFwpmEngineHandle engineHandle,
          ulong endpointId,
          out SafeFwpmMemoryBuffer properties // FWPS_ALE_ENDPOINT_PROPERTIES0** 
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpsAleEndpointGetSecurityInfo0(
          SafeFwpmEngineHandle engineHandle,
          SecurityInformation securityInfo,
          IntPtr sidOwner,
          IntPtr sidGroup,
          IntPtr dacl,
          IntPtr sacl,
          out SafeFwpmMemoryBuffer securityDescriptor // PSECURITY_DESCRIPTOR* 
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error IkeextSaCreateEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            SafeBuffer enumTemplate, // const IKEEXT_SA_ENUM_TEMPLATE0* 
            out IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error IkeextSaDestroyEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error IkeextSaEnum1(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle,
            int numEntriesRequested,
            out SafeFwpmMemoryBuffer entries, // IKEEXT_SA_DETAILS1*** 
            out int numEntriesReturned
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error IkeextSaGetById1(
            SafeFwpmEngineHandle engineHandle,
            ulong id,
            OptionalGuid saLookupContext,
            out SafeFwpmMemoryBuffer sa // IKEEXT_SA_DETAILS1** 
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmNetEventCreateEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            SafeBuffer enumTemplate, // const FWPM_NET_EVENT_ENUM_TEMPLATE0* 
            out IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmNetEventEnum2(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle,
            int numEntriesRequested,
            out SafeFwpmMemoryBuffer entries, // FWPM_NET_EVENT2*** 
            out int numEntriesReturned
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmNetEventDestroyEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmNetEventSubscribe1(
            SafeFwpmEngineHandle engineHandle,
            in FWPM_NET_EVENT_SUBSCRIPTION0 subscription,
            IntPtr callback,
            IntPtr context,
            out IntPtr eventsHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error FwpmNetEventUnsubscribe0(
            SafeFwpmEngineHandle engineHandle,
            IntPtr eventsHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error IPsecSaContextCreateEnumHandle0(
          SafeFwpmEngineHandle engineHandle,
          SafeBuffer enumTemplate, // const IPSEC_SA_CONTEXT_ENUM_TEMPLATE0*
          out IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error IPsecSaContextEnum1(
          SafeFwpmEngineHandle engineHandle,
          IntPtr enumHandle,
          int numEntriesRequested,
          out SafeFwpmMemoryBuffer entries, // IPSEC_SA_CONTEXT1***
          out int numEntriesReturned
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error IPsecSaContextDestroyEnumHandle0(
            SafeFwpmEngineHandle engineHandle,
            IntPtr enumHandle
        );

        [DllImport("Fwpuclnt.dll", CharSet = CharSet.Unicode)]
        internal static extern Win32Error IPsecSaContextGetById1(
          SafeFwpmEngineHandle engineHandle,
          ulong id,
          out SafeFwpmMemoryBuffer saContext // IPSEC_SA_CONTEXT1**
        );
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member